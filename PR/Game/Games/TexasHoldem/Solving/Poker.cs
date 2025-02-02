using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Common;
using Game.Games.TexasHoldem.Model;
using Game.Games.TexasHoldem.Utils;
using Game.Interfaces;
using Game.MultiRegionMatchers;
using Game.RegionMatchers;
using PT.Algorithm;
using PT.Algorithm.Model;
using PT.Poker.Model;
using PT.Poker.Resolving;

namespace Game.Games.TexasHoldem.Solving
{
    public class Poker
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType!);

        private Flop _flop;
        private Turn _turn;
        private River _river;
        private PlayerCards _playerCards;
        private Position _position;
        private Opponent _opponent;
        private Stack _stack;
        private Nickname _nickname;
        private int _numPlayers;
        private IDictionary<string, ReconResult> _state;
        private Decision _decision;
        private Pot _pot;
        private MatchResults _lastMatchResults;

        private PokerResults _prevPokerResults;
        private PokerPhase _previousPhase;
        private StartingBets _startingBets;
        private readonly List<PlayerAction> _gameActions = new();
        private PokerPosition? _lastPokerPosition;
        private decimal[] _currentStreetContributions;
        private decimal _currentStreetHighestBet;
        private IReadOnlyList<bool> _previousOpponentsInGame;
        private PokerActionType[] _lastActionThisStreet;

        public Poker(Board board)
        {
            Board = board;
            InitializeMatchers();
        }

        public void SetState(IDictionary<string, ReconResult> state)
        {
            _state = state;
        }

        public Board Board { get; }

        private int NumPlayers => _numPlayers;

        public StartingBets StartingBets => _startingBets;

        public List<PlayerAction> GameActions => _gameActions;

        public PokerDebugFlags DebugFlags { get; set; } = PokerDebugFlags.None;

        private void InitializeMatchers()
        {
            var settings = new PokerBoardSettingsParser(Board);
            _numPlayers = settings.Players;

            _flop = new Flop(Board);
            _turn = new Turn(Board);
            _river = new River(Board);
            _playerCards = new PlayerCards(Board);
            _position = new Position(Board, _numPlayers);
            _opponent = new Opponent(Board, _numPlayers - 1);
            _stack = new Stack(_numPlayers);
            _nickname = new Nickname(_numPlayers);
            _decision = new Decision(Board);
            _pot = new Pot();
            _currentStreetContributions = new decimal[_numPlayers];
        }

        public List<RegionSpec> GetRegionSpecs()
        {
            List<RegionSpec> regionSpecs =
            [
                _flop.GetRegionSpec(),
                _turn.GetRegionSpec(),
                _river.GetRegionSpec(),
                _playerCards.GetRegionSpec(),
                _decision.GetRegionSpec(),
            ];
            regionSpecs.AddRange(_position.GetRegionSpecs());
            regionSpecs.AddRange(_opponent.GetRegionSpecs());
            regionSpecs.AddRange(_stack.GetRegionSpecs());
            regionSpecs.AddRange(_nickname.GetRegionSpecs());
            regionSpecs.AddRange(_pot.GetRegionSpecs());
            return regionSpecs;
        }

        private ReconResults GetReconResults()
        {
            var playerResult = GetResult(nameof(PlayerCards));
            var flopResult = GetResult(nameof(Flop));
            var turnResult = GetResult(nameof(Turn));
            var riverResult = GetResult(nameof(River));
            var positionResults = GetResultsPrefixed(nameof(Position)).ToList();
            var opponentResults = GetResultsPrefixed(nameof(Opponent)).ToList();
            var stackResults = GetResultsPrefixed(nameof(Stack)).ToList();
            var nicknameResults = GetResultsPrefixed(nameof(Nickname)).ToList();
            var decisionResult = GetResult(nameof(Decision));
            var potResult = GetResult(nameof(Pot));

            return new ReconResults(playerResult, flopResult, turnResult, riverResult, positionResults,
                opponentResults, stackResults, nicknameResults, decisionResult, potResult);
        }

        private PokerPhase DeterminePokerPhase(List<Card> flopCards, List<Card> turnCards, List<Card> riverCards)
        {
            if (riverCards.Any())
                return PokerPhase.River;
            if (turnCards.Any())
                return PokerPhase.Turn;
            if (flopCards.Any())
                return PokerPhase.Flop;
            return PokerPhase.Preflop;
        }

        public PokerResults Solve()
        {
            var reconResults = GetReconResults();
            var playerCards = _playerCards.Match(reconResults.PlayerResult);
            var flopCards = _flop.Match(reconResults.FlopResult);
            var turnCards = _turn.Match(reconResults.TurnResult);
            var riverCards = _river.Match(reconResults.RiverResult);
            Place position = _position.Match(reconResults.PositionResults);
            Place opponents = _opponent.Match(reconResults.OpponentResults);
            List<decimal?> stack = _stack.Match(reconResults.StackResults);
            List<string> nicknames = _nickname.Match(reconResults.NicknameResults);
            var isDecision = _decision.Match(reconResults.DecisionResult);
            var pot = _pot.Match([reconResults.PotResult]);
            var numPlayers = NumPlayers;
            
            var matchResults =
                new MatchResults(playerCards, flopCards, turnCards, riverCards,
                    position, opponents, nicknames, stack, isDecision, pot);

            var opponentsInGame = opponents.Places(numPlayers - 1);
            var phase = DeterminePokerPhase(flopCards, turnCards, riverCards);
            if (_previousPhase == PokerPhase.None)
            {
                _previousPhase = phase;
            }

            MonteCarloResult? monteCarloResult = null;
            PokerLayouts? bestLayout = null;
            PokerPosition? pokerPosition = null;
            var newPosition = position.GetPokerPosition(numPlayers);
            pokerPosition = newPosition;

            // Check if position has changed
            if (_lastPokerPosition != null && newPosition != _lastPokerPosition)
            {
                // Position changed, clear game actions
                _gameActions.Clear();
                _previousPhase = PokerPhase.None;
                _startingBets = null;
            }

            _lastPokerPosition = newPosition;

            if (playerCards.Count != 0)
            {
                int countPlayers = opponents.Count + 1; // +1 for the player
                monteCarloResult = ComputeMonteCarloResult(playerCards,
                    flopCards.Union(turnCards).Union(riverCards).ToList(),
                    countPlayers);
                var allCards = playerCards.Union(flopCards).Union(turnCards).Union(riverCards).ToArray();
                var layoutResolver = new LayoutResolver(new CardLayout(allCards));
                bestLayout = layoutResolver.PokerLayout;
            }

            bool isCorrectPot = true;
            

            var pokerResult = new PokerResults(
                reconResults,
                matchResults,
                monteCarloResult,
                bestLayout,
                pokerPosition,
                ImmutableList<bool>.Empty.AddRange(opponentsInGame),
                phase,
                isCorrectPot);

            // Initialize snapshots if they're not set
            if (_prevPokerResults == null)
            {
                _prevPokerResults = pokerResult;
            }

            if (_previousOpponentsInGame == null)
            {
                _previousOpponentsInGame = opponentsInGame.ToArray();
            }

            if (_lastActionThisStreet == null)
            {
                _lastActionThisStreet = new PokerActionType[numPlayers];
            }

            if (playerCards.Count != 0 && (_lastMatchResults == null || !_lastMatchResults.Equals(matchResults)))
            {
                if (DebugFlags.HasFlag(PokerDebugFlags.StateResults))
                {
                    Log.Debug(System.Text.Json.JsonSerializer.Serialize(
                        _state.OrderBy(kvp => kvp.Key)
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Results)));
                }

                if (DebugFlags.HasFlag(PokerDebugFlags.MatchResults))
                {
                    Log.Debug(System.Text.Json.JsonSerializer.Serialize(
                        new
                        {
                            PlayerCards = matchResults.PlayerCards.Select(c => c.ToCString()),
                            Flop = matchResults.Flop.Select(c => c.ToCString()),
                            Turn = matchResults.Turn.Select(c => c.ToCString()),
                            River = matchResults.River.Select(c => c.ToCString()),

                            OpponentCount = matchResults.Opponent.Count,
                            matchResults.Stacks,
                            matchResults.IsPlayerDecision,
                            matchResults.Pot,

                            PokerPosition = pokerPosition?.ToDisplayString() ?? "",
                            OpponentsInGame = opponentsInGame
                        }
                    ));
                }

                _lastMatchResults = matchResults;

                // Always infer actions since the start of the phase
                if (_prevPokerResults != pokerResult)
                {
                    var gameBets = InferActions(
                        _previousPhase,
                        _prevPokerResults.MatchResults.Stacks,
                        matchResults.Stacks,
                        opponentsInGame,
                        _previousOpponentsInGame,
                        matchResults.Position.Pos,
                        numPlayers,
                        _startingBets,
                        _currentStreetContributions,
                        ref _currentStreetHighestBet,
                        _lastActionThisStreet);
                    if (_state.TryGetValue("_id", out var value))
                    {
                        gameBets = gameBets with
                        {
                            Actions = gameBets.Actions.Select(x => x with {Id = value.Result})
                                .ToImmutableList()
                        };
                    }
                    var gameBetsActions = gameBets.Actions;

                    if (DebugFlags.HasFlag(PokerDebugFlags.ActionRecognition))
                    {
                        Log.Debug(
                            $"Ante={gameBets.StartingBets.Ante}, SmallBlind={gameBets.StartingBets.SmallBlind} BigBlind={gameBets.StartingBets.BigBlind}");
                    }

                    foreach (var gameBetsAction in gameBetsActions)
                    {
                        if (gameBetsAction.ActionType != PokerActionType.Check)
                        {
                            for (var i = _gameActions.Count - 1; i >= 0; i--)
                            {
                                var a = _gameActions[i];
                                if (a.PlayerIndex == gameBetsAction.PlayerIndex &&
                                    a.Phase == phase &&
                                    a.ActionType == PokerActionType.Check)
                                {
                                    _gameActions.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                    }

                    _gameActions.AddRange(gameBetsActions);
                    _startingBets = gameBets.StartingBets;

                    //Log recognized actions
                    if (gameBetsActions.Any() && DebugFlags.HasFlag(PokerDebugFlags.ActionRecognition))
                    {
                        Log.Debug("=== Actions recognized since the start of the phase: ===");
                        foreach (var action in gameBetsActions)
                        {
                            Log.Debug(action.ToString());
                        }
                    }
                }
            }
            
            // Calculate total contributions
            decimal totalContributions = _gameActions.Sum(action => action.Amount);

            if (matchResults.Pot != null)
                isCorrectPot = Math.Abs(totalContributions - matchResults.Pot.Value) < 0.01m;

            if (!isCorrectPot)
            {
                var potDiff = pokerResult.MatchResults.Pot - _prevPokerResults.MatchResults.Pot;
                if (potDiff.HasValue)
                {
                    var newGameActionsAmounts = _gameActions.Select(x => x.Amount).ToList();
                    if (newGameActionsAmounts.Any())
                    {
                        newGameActionsAmounts[^1] = potDiff.Value;
                        var totalContributions2 = newGameActionsAmounts.Sum(x => x);
                        if (matchResults.Pot != null)
                        {
                            var isCorrectPot2 = Math.Abs(totalContributions2 - matchResults.Pot.Value) < 0.01m;
                            if (isCorrectPot2)
                            {
                                _gameActions[^1] = new PlayerAction(Amount: potDiff.Value, Id: _gameActions[^1].Id,
                                    Phase: _gameActions[^1].Phase,
                                    ActionType: potDiff == 0 ? PokerActionType.Check : _gameActions[^1].ActionType,
                                    PlayerIndex: _gameActions[^1].PlayerIndex);
                            }
                        }
                    }
                }
            }

            pokerResult = pokerResult with {IsCorrectPot = isCorrectPot};

            _prevPokerResults = pokerResult;
            if (phase != _previousPhase)
            {
                Array.Clear(_currentStreetContributions);
                _currentStreetHighestBet = 0;
                _lastActionThisStreet = new PokerActionType[numPlayers];
            }

            _previousPhase = phase;
            _previousOpponentsInGame = opponentsInGame.ToArray();

            return pokerResult;
        }

        public Dictionary<string, IResultPresenter> GetPresenters()
        {
            return new Dictionary<string, IResultPresenter>
            {
                {nameof(PlayerCards), _playerCards.GetPresenter()},
                {nameof(Flop), _flop.GetPresenter()},
                {nameof(Turn), _turn.GetPresenter()},
                {nameof(River), _river.GetPresenter()},
                {nameof(Position), _position.GetPresenter()},
                {nameof(Opponent), _opponent.GetPresenter()},
                {nameof(Stack), _stack.GetPresenter()},
                {nameof(Nickname), _nickname.GetPresenter()},
                {nameof(Decision), _decision.GetPresenter()},
                {nameof(Pot), _pot.GetPresenter()}
            };
        }

        private IEnumerable<ReconResult> GetResultsPrefixed(string name)
        {
            return _state.Where(x => x.Key.StartsWith(name)).OrderBy(x => x.Key)
                .Select(x => x.Value).ToList();
        }

        private ReconResult GetResult(string name)
        {
            _state.TryGetValue(name, out var result);
            return result;
        }

        private static MonteCarloResult ComputeMonteCarloResult(List<Card> myCards, List<Card> boardCards,
            int numOfPlayers)
        {
            RandomSetDefinition arg = new RandomSetDefinition
            {
                MyLayout = new CardLayout(myCards.ToArray()),
                NumOfPlayers = numOfPlayers,
                Board = boardCards.ToArray()
            };
            MonteCarlo<CardSet, RandomSetDefinition> monteCarlo =
                new MonteCarlo<CardSet, RandomSetDefinition>(250, arg);

            MonteCarloResult result = monteCarlo.Solve();
            return result;
        }

        private static GameBets InferActions(
            PokerPhase phase,
            IList<decimal?> previousStacks,
            IList<decimal?> currentStacks,
            IReadOnlyList<bool> opponentsInGame,
            IReadOnlyList<bool> previousOpponentsInGame,
            int dealerPosition,
            int numPlayers,
            StartingBets startingBets,
            IList<decimal> currentStreetContributions,
            ref decimal currentStreetHighestBet,
            PokerActionType[] lastActionThisStreet)
        {
            if (previousStacks.Count == 0) return new GameBets([], new StartingBets(0, 0, 0));
            var actions = new List<PlayerAction>();

            // Calculate stack differences
            var contributions = new decimal[previousStacks.Count];
            for (int i = 0; i < previousStacks.Count; i++)
            {
                if (previousStacks[i].HasValue && currentStacks[i].HasValue)
                    contributions[i] = previousStacks[i].Value - currentStacks[i].Value;
            }

            // First pass - identify ante amount from players not in game and blind amounts
            decimal ante = 0;
            int sbPos = (dealerPosition + 1) % numPlayers;
            int bbPos = (dealerPosition + 2) % numPlayers;

            if (startingBets == null)
            {
                decimal smallBlind = 0;
                decimal bigBlind = 0;
                for (int i = 0; i < contributions.Length; i++)
                {
                    var playerPos = (i + 1) % numPlayers;
                    var isSmallBlind = playerPos == sbPos;
                    var isBigBlind = playerPos == bbPos;
                    var contribution = contributions[i];
                    if (!isSmallBlind && !isBigBlind && ante == 0 && contribution > 0)
                    {
                        ante = contribution;
                    }

                    if (isSmallBlind)
                    {
                        smallBlind = contribution;
                    }

                    if (isBigBlind)
                    {
                        bigBlind = contribution;
                    }
                }

                smallBlind -= ante;
                bigBlind -= ante;

                if (smallBlind < 0)
                {
                    smallBlind = bigBlind / 2;
                }

                if (bigBlind < 0)
                {
                    bigBlind = smallBlind * 2;
                }

                startingBets = new StartingBets(ante, smallBlind, bigBlind);

                actions.Add(new PlayerAction(dealerPosition % numPlayers + 1, PokerActionType.Put, smallBlind + ante,
                    phase));
                actions.Add(new PlayerAction((dealerPosition + 1) % numPlayers + 1, PokerActionType.Put,
                    bigBlind + ante,
                    phase));
                for (int i = 0; i < numPlayers - 2; i++)
                {
                    actions.Add(new PlayerAction((dealerPosition + 2 + i) % numPlayers + 1,
                        PokerActionType.Put, ante, phase));
                }
            }

            if (phase == PokerPhase.Preflop && currentStreetHighestBet < startingBets.BigBlind)
            {
                currentStreetHighestBet = startingBets.BigBlind;
            }

            // Process actions for active players
            if (phase != PokerPhase.None)
            {
                for (int k = 0; k < numPlayers; k++)
                {
                    var i = (dealerPosition + k) % numPlayers;
                    bool wasInGame = (i == 0) || (previousOpponentsInGame?[i - 1] ?? false);
                    bool isInGame = (i == 0) || opponentsInGame[i - 1];

                    // 1) FOLD: if seat was in but now is out
                    if (wasInGame && !isInGame)
                    {
                        actions.Add(new PlayerAction(i + 1, PokerActionType.Fold, 0, phase));
                        lastActionThisStreet[i] = PokerActionType.Fold;
                        continue;
                    }

                    // 2) STILL IN
                    if (wasInGame)
                    {
                        decimal amountPutIn = contributions[i];
                        bool isAllIn = currentStacks[i] == 0;

                        if (amountPutIn > 0)
                        {
                            // Existing logic: Bet/Call/Raise/AllIn
                            currentStreetContributions[i] += amountPutIn;

                            var actionType = DetermineActionType(
                                currentStreetHighestBet,
                                currentStreetContributions[i],
                                isAllIn
                            );

                            actions.Add(new PlayerAction(i + 1, actionType, amountPutIn, phase));
                            lastActionThisStreet[i] = actionType;

                            // Possibly update highest bet
                            if (currentStreetContributions[i] > currentStreetHighestBet)
                                currentStreetHighestBet = currentStreetContributions[i];
                        }
                        else
                        {
                            bool alreadyChecked = lastActionThisStreet[i] != PokerActionType.None;

                            bool noOutstandingBet = currentStreetContributions[i] >= currentStreetHighestBet;

                            if (!alreadyChecked && noOutstandingBet)
                            {
                                actions.Add(new PlayerAction(i + 1, PokerActionType.Check, 0, phase));
                                lastActionThisStreet[i] = PokerActionType.Check;
                            }
                        }
                    }
                }
            }

            return new GameBets(actions.ToImmutableList(), startingBets);
        }

        private static PokerActionType DetermineActionType(
            decimal currentStreetHighestBet,
            decimal playerContributionInThisStreet,
            bool isAllIn
        )
        {
            if (isAllIn)
                return PokerActionType.AllIn;

            if (currentStreetHighestBet == 0)
            {
                return PokerActionType.Bet;
            }

            if (playerContributionInThisStreet <= currentStreetHighestBet)
            {
                return PokerActionType.Call;
            }

            if (playerContributionInThisStreet > currentStreetHighestBet)
            {
                return PokerActionType.Raise;
            }

            throw new InvalidOperationException("Unable to determine action type.");
        }
    }
}