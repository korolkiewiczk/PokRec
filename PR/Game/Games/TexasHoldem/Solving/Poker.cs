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
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
        private List<PlayerAction> _gameActions = new();
        private PokerPosition? _lastPokerPosition;

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

        public int NumPlayers => _numPlayers;

        public StartingBets StartingBets => _startingBets;

        public List<PlayerAction> GameActions => _gameActions;

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
            var position = _position.Match(reconResults.PositionResults);
            var opponents = _opponent.Match(reconResults.OpponentResults);
            var stack = _stack.Match(reconResults.StackResults);
            var nicknames = _nickname.Match(reconResults.NicknameResults);
            var isDecision = _decision.Match(reconResults.DecisionResult);
            var pot = _pot.Match([reconResults.PotResult]);

            var matchResults =
                new MatchResults(playerCards, flopCards, turnCards, riverCards,
                    position, opponents, nicknames, stack, isDecision, pot);

            var opponentsInGame = opponents.Places(NumPlayers - 1);
            var phase = DeterminePokerPhase(flopCards, turnCards, riverCards);
            if (_previousPhase == PokerPhase.None)
            {
                _previousPhase = phase;
            }

            MonteCarloResult? monteCarloResult = null;
            PokerLayouts? bestLayout = null;
            PokerPosition? pokerPosition = null;
            var newPosition = position.GetPokerPosition(NumPlayers);
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

            var pokerResult = new PokerResults(
                reconResults,
                matchResults,
                monteCarloResult,
                bestLayout,
                pokerPosition,
                ImmutableList<bool>.Empty.AddRange(opponentsInGame),
                phase);

            // Initialize snapshots if they're not set
            if (_prevPokerResults == null)
            {
                _prevPokerResults = pokerResult;
            }

            if (playerCards.Count != 0 && (_lastMatchResults == null || !_lastMatchResults.Equals(matchResults)))
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
                _lastMatchResults = matchResults;

                // Always infer actions since the start of the phase
                if (_prevPokerResults != pokerResult)
                {
                    var gameBets = InferActions(
                        _previousPhase,
                        _prevPokerResults.MatchResults.Pot,
                        _prevPokerResults.MatchResults.Stacks,
                        matchResults.Stacks,
                        opponentsInGame,
                        matchResults.Position.Pos,
                        NumPlayers,
                        _startingBets);
                    var gameBetsActions = gameBets.Actions;
                    Log.Debug(
                        $"Ante={gameBets.StartingBets.Ante}, SmallBlind={gameBets.StartingBets.SmallBlind} BigBlind={gameBets.StartingBets.BigBlind}");
                    _gameActions.AddRange(gameBetsActions);
                    _startingBets = gameBets.StartingBets;

                    // Log recognized actions
                    if (gameBetsActions.Any())
                    {
                        Log.Debug("=== Actions recognized since the start of the phase: ===");
                        foreach (var action in gameBetsActions)
                        {
                            Log.Debug(action.ToString());
                        }
                    }
                }
            }

            _prevPokerResults = pokerResult;
            _previousPhase = phase;

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
            PokerPhase previousPhase,
            decimal previousPot,
            IList<decimal?> previousStacks,
            IList<decimal?> currentStacks,
            IReadOnlyList<bool> opponentsInGame,
            int dealerPosition,
            int numPlayers,
            StartingBets startingBets)
        {
            var actions = new List<PlayerAction>();
            var phase = previousPhase;

            // Calculate pot delta, assuming 0 starting pot for preflop
            decimal startingPot = phase == PokerPhase.Preflop ? 0 : previousPot;

            // Calculate stack differences
            decimal[] contributions = new decimal[previousStacks.Count];
            for (int i = 0; i < previousStacks.Count; i++)
            {
                if (previousStacks[i].HasValue && currentStacks[i].HasValue)
                    contributions[i] = previousStacks[i].Value - currentStacks[i].Value;
                else
                    contributions[i] = 0;
            }

            // First pass - identify ante amount from players not in game and blind amounts
            decimal ante = 0;

            // Identify SB and BB positions based on dealer position
            int sbPos = (dealerPosition + 1) % numPlayers;
            int bbPos = (dealerPosition + 2) % numPlayers;

            if (startingBets == null)
            {
                decimal smallBlind = 0;
                decimal bigBlind = 0;
                // First identify ante from players not in game
                for (int i = 0; i < contributions.Length; i++) // Start from 1 to skip player
                {
                    bool isSmallBlind = (i + 1) % numPlayers == sbPos; // Is Small Blind
                    bool isBigBlind = (i + 1) % numPlayers == bbPos; // Is Big Blind
                    if (!isSmallBlind && !isBigBlind && ante == 0 && contributions[i] > 0)
                    {
                        ante = contributions[i];
                    }

                    if (isSmallBlind)
                    {
                        smallBlind = contributions[i];
                    }

                    if (isBigBlind)
                    {
                        bigBlind = contributions[i];
                    }
                }

                startingBets = new StartingBets(ante, smallBlind - ante, bigBlind - ante);
            }

            // Process actions for active players
            for (int i = 0; i < contributions.Length; i++)
            {
                // For player (i == 0) always process, for opponents check if they're in game
                bool isPlayer = i == 0;
                bool isOpponentInGame = !isPlayer && opponentsInGame[i - 1]; // Adjust index for OpponentsInGame

                // Skip if it's an opponent not in game or no contribution
                if (!isPlayer && !isOpponentInGame || contributions[i] <= 0)
                    continue;

                bool isAllIn = currentStacks[i] == 0;
                decimal actualAmount = ante > 0 ? contributions[i] - ante : contributions[i];
                var isSmallBlind = phase == PokerPhase.None && (i + 1) % numPlayers == sbPos;
                var isBigBlind = phase == PokerPhase.None && (i + 1) % numPlayers == bbPos;
                if (actualAmount == 0 || isSmallBlind || isBigBlind) continue;

                var actionType = DetermineActionType(startingPot, isAllIn);

                actions.Add(new PlayerAction(i + 1, actionType, actualAmount,
                    phase));
            }

            return new GameBets(actions.ToImmutableList(), startingBets);
        }

        private static PokerActionType DetermineActionType(
            decimal startingPot,
            bool isAllIn)
        {
            if (isAllIn) return PokerActionType.AllIn;
            return startingPot != 0 ? PokerActionType.Bet : PokerActionType.Call;
        }
    }
}