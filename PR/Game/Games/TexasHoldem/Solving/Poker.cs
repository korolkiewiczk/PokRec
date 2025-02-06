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
        private Place _lastPosition;
        private decimal[] _currentStreetContributions;
        private decimal _currentStreetHighestBet;
        private IList<bool> _previousOpponentsInGame;
        private PokerActionType[] _lastActionThisStreet;
        private List<int> _activeIndices;
        private List<int> _prevActiveIndices;

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
            if (riverCards.Count != 0)
                return PokerPhase.River;
            if (turnCards.Count != 0)
                return PokerPhase.Turn;
            if (flopCards.Count != 0)
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

            var phase = DeterminePokerPhase(flopCards, turnCards, riverCards);
            if (_previousPhase == PokerPhase.None)
            {
                _previousPhase = phase;
            }

            // Check if position has changed - new game then
            if (_lastPosition != null && position != _lastPosition)
            {
                ClearPropertiesOnNewStreet();
            }

            _lastPosition = position;

            _activeIndices ??= FilterActiveIndices(stack);

            var numPlayers = _activeIndices.Count;

            stack = _activeIndices.Select(i => stack[i]).ToList();

            if (_prevPokerResults != null)
            {
                var remappedStacks = RemapPreviousStacks(stack, _activeIndices, _prevActiveIndices,
                    _prevPokerResults.MatchResults.Stacks);
                _prevPokerResults = _prevPokerResults with
                {
                    MatchResults = _prevPokerResults.MatchResults with {Stacks = remappedStacks}
                };
            }

            _prevActiveIndices = _activeIndices;

            nicknames = _activeIndices.Select(i => nicknames[i]).ToList();

            position = RemapPlace(position, _activeIndices, _numPlayers);
            opponents = RemapPlace(opponents, _activeIndices.Where(x => x != 0).Select(x => x - 1).ToList(),
                _numPlayers - 1);

            PokerPosition? pokerPosition = position.GetPokerPosition(numPlayers);

            var matchResults =
                new MatchResults(playerCards, flopCards, turnCards, riverCards,
                    position, opponents, nicknames, stack, isDecision, pot);

            var opponentsInGame = opponents.Places(numPlayers - 1);

            var (monteCarloResult, bestLayout) =
                SolvePlayerLayout(playerCards, opponents, flopCards, turnCards, riverCards);

            var pokerResult = new PokerResults(
                reconResults,
                matchResults,
                monteCarloResult,
                bestLayout,
                pokerPosition,
                opponentsInGame.ToImmutableList(),
                phase,
                true);

            // Initialize snapshots if they're not set
            _prevPokerResults ??= pokerResult;
            _previousOpponentsInGame ??= opponentsInGame.ToList();
            _lastActionThisStreet ??= new PokerActionType[numPlayers];

            if (_lastMatchResults == null || !_lastMatchResults.Equals(matchResults))
            {
                DebuggingLogs(matchResults, pokerPosition, opponentsInGame);

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

                    gameBets = FillWithId(gameBets);

                    var gameBetsActions = gameBets.Actions;

                    DebuggingLogs(gameBets);

                    RemoveRedundantChecks(gameBetsActions, phase);

                    _gameActions.AddRange(gameBetsActions);
                    _startingBets = gameBets.StartingBets;

                    DebuggingLogs(gameBetsActions);
                }
            }

            var isCorrectPot = IsCorrectPotWithFixActions(matchResults);

            pokerResult = pokerResult with {IsCorrectPot = isCorrectPot};

            _prevPokerResults = pokerResult;
            if (phase != _previousPhase)
            {
                ClearPropertiesOnNewPhase(numPlayers);
            }

            _previousPhase = phase;
            _previousOpponentsInGame = opponentsInGame.ToList();

            return pokerResult;
        }

        private static List<int> FilterActiveIndices(IEnumerable<decimal?> stack)
        {
            return stack
                .Select((s, index) => new {StackValue = s, Index = index})
                .Where(x => x.StackValue.HasValue)
                .Select(x => x.Index)
                .ToList();
        }

        private void ClearPropertiesOnNewPhase(int numPlayers)
        {
            Array.Clear(_currentStreetContributions);
            _currentStreetHighestBet = 0;
            _lastActionThisStreet = new PokerActionType[numPlayers];
        }


        private GameBets FillWithId(GameBets gameBets)
        {
            if (_state.TryGetValue("_id", out var value))
            {
                gameBets = gameBets with
                {
                    Actions = gameBets.Actions.Select(x => x with {Id = value.Result})
                        .ToImmutableList()
                };
            }

            return gameBets;
        }

        private static List<decimal?> RemapPreviousStacks(
            List<decimal?> stack,
            List<int> activeIndices,
            List<int> prevActiveIndices,
            List<decimal?> prevStacks)
        {
            if (prevStacks == null)
            {
                return stack.ToList();
            }

            if (stack.Count < prevStacks.Count)
            {
                return activeIndices.Select(i => prevStacks[i]).ToList();
            }

            if (stack.Count > prevStacks.Count && prevActiveIndices != null)
            {
                return activeIndices
                    .Select(activeIndex => prevActiveIndices.IndexOf(activeIndex))
                    .Select((prevIndex, i) => prevIndex != -1 ? prevStacks[prevIndex] : stack[i]).ToList();
            }

            return prevStacks;
        }

        private void RemoveRedundantChecks(IEnumerable<PlayerAction> gameBetsActions, PokerPhase phase)
        {
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
        }

        private void ClearPropertiesOnNewStreet()
        {
            _gameActions.Clear();
            _previousPhase = PokerPhase.None;
            _startingBets = null;
            _activeIndices = null;
            _previousOpponentsInGame = null;
        }

        private bool IsCorrectPotWithFixActions(MatchResults matchResults)
        {
            decimal totalContributions = _gameActions.Sum(action => action.Amount);

            bool isCorrectPot = true;
            if (matchResults.Pot != null)
            {
                isCorrectPot = Math.Abs(totalContributions - matchResults.Pot.Value) < 0.01m;
            }

            if (!isCorrectPot)
            {
                var potDiff = matchResults.Pot - _prevPokerResults.MatchResults.Pot;
                if (potDiff.HasValue)
                {
                    var newGameActionsAmounts = _gameActions.Select(x => x.Amount).ToList();
                    if (newGameActionsAmounts.Count != 0)
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

            return isCorrectPot;
        }

        private static (MonteCarloResult? monteCarloResult, PokerLayouts? bestLayout) SolvePlayerLayout(
            List<Card> playerCards,
            Place opponents, IList<Card> flopCards, IList<Card> turnCards, IList<Card> riverCards)
        {
            MonteCarloResult? monteCarloResult = null;
            PokerLayouts? bestLayout = null;
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

            return (monteCarloResult, bestLayout);
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

        private static MonteCarloResult ComputeMonteCarloResult(IEnumerable<Card> myCards, IEnumerable<Card> boardCards,
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
            IList<bool> opponentsInGame,
            IList<bool> previousOpponentsInGame,
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

            if (startingBets == null)
            {
                var (ante, smallBlind, bigBlind) = DeduceBlindsBasingOnPosition(dealerPosition, contributions);

                if (smallBlind < 0 || bigBlind < 0)
                {
                    (ante, smallBlind, bigBlind) = DeduceBlindsBasingOnContributions(contributions);
                    startingBets = new StartingBets(ante, smallBlind, bigBlind);
                    actions.AddRange(contributions.Select((x, i) =>
                        new PlayerAction(i + 1, PokerActionType.Put, x, PokerPhase.None)));
                }
                else
                {
                    startingBets = new StartingBets(ante, smallBlind, bigBlind);

                    actions.Add(new PlayerAction(dealerPosition % numPlayers + 1, PokerActionType.Put,
                        smallBlind + ante,
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

                            // Possibly update the highest bet
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

        private static Place RemapPlace(Place p, IList<int> active, int originalNumPlayers)
        {
            var remappedPlace = new Place();
            var origPlaceBooleans = p.Places(originalNumPlayers);

            for (int i = 0; i < origPlaceBooleans.Count; i++)
            {
                if (origPlaceBooleans[i] && active.Contains(i))
                {
                    var newIndex = active.IndexOf(i);
                    remappedPlace.Add(newIndex);
                }
            }

            return remappedPlace;
        }

        private static (decimal ante, decimal smallBlind, decimal bigBlind) DeduceBlindsBasingOnPosition(
            int dealerPosition,
            decimal[] contributions)
        {
            int sbPos = (dealerPosition + 1) % contributions.Length;
            int bbPos = (dealerPosition + 2) % contributions.Length;
            decimal ante = 0;
            decimal smallBlind = 0;
            decimal bigBlind = 0;
            for (int i = 0; i < contributions.Length; i++)
            {
                var playerPos = (i + 1) % contributions.Length;
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

            return (ante, smallBlind, bigBlind);
        }

        private static (decimal Ante, decimal SmallBlind, decimal BigBlind) DeduceBlindsBasingOnContributions(
            decimal[] contributions)
        {
            var validContribs = (contributions.Any(x => x > 0)
                ? contributions.Where(x => x > 0)
                : contributions).ToArray();

            var groups = validContribs.GroupBy(x => x).ToList();
            decimal ante = groups
                .Where(g => g.Key > 0 && g.Count() > 1)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .Select(g => g.Key)
                .FirstOrDefault();

            var extras = validContribs.Distinct()
                .Where(x => x > ante)
                .Select(x => x - ante)
                .OrderBy(x => x)
                .ToList();

            var (smallBlind, bigBlind) = extras.Count switch
            {
                0 => (0m, 0m),
                _ => (extras.First(), 2 * extras.First())
            };

            return (ante, smallBlind, bigBlind);
        }

        #region Debbuging methods

        private void DebuggingLogs(MatchResults matchResults, PokerPosition? pokerPosition, List<bool> opponentsInGame)
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
        }

        private void DebuggingLogs(GameBets gameBets)
        {
            if (DebugFlags.HasFlag(PokerDebugFlags.ActionRecognition))
            {
                Log.Debug(
                    $"Ante={gameBets.StartingBets.Ante}, SmallBlind={gameBets.StartingBets.SmallBlind} BigBlind={gameBets.StartingBets.BigBlind}");
            }
        }

        private void DebuggingLogs(IEnumerable<PlayerAction> gameBetsActions)
        {
            //Log recognized actions
            var playerActions = gameBetsActions.ToList();
            if (playerActions.Count != 0 && DebugFlags.HasFlag(PokerDebugFlags.ActionRecognition))
            {
                Log.Debug("=== Actions recognized since the start of the phase: ===");
                foreach (var action in playerActions)
                {
                    Log.Debug(action.ToString());
                }
            }
        }

        #endregion
    }
}