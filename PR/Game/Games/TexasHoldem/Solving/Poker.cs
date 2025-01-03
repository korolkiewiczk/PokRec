using System.Collections.Generic;
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
        }

        public List<RegionSpec> GetRegionSpecs()
        {
            List<RegionSpec> regionSpecs =
            [
                _flop.GetRegionSpec(),
                _turn.GetRegionSpec(),
                _river.GetRegionSpec(),
                _playerCards.GetRegionSpec()
            ];
            regionSpecs.AddRange(_position.GetRegionSpecs());
            regionSpecs.AddRange(_opponent.GetRegionSpecs());
            regionSpecs.AddRange(_stack.GetRegionSpecs());
            regionSpecs.AddRange(_nickname.GetRegionSpecs());
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

            return new ReconResults(playerResult, flopResult, turnResult, riverResult, positionResults,
                opponentResults, stackResults, nicknameResults);
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
            var nicknameToStack = new Dictionary<string, int>();
            for (int i = 0; i < nicknames.Count && i < stack.Count; i++)
            {
                nicknameToStack[nicknames[i]] = (int) (stack[i] ?? 0);
            }

            var matchResults =
                new MatchResults(playerCards, flopCards, turnCards, riverCards,
                    position, opponents, nicknameToStack);

            MonteCarloResult? monteCarloResult = null;
            PokerLayouts? bestLayout = null;
            PokerPosition? pokerPosition = null;
            if (playerCards.Count != 0)
            {
                int countPlayers = opponents.Count + 1; // +1 for the player
                monteCarloResult = ComputeMonteCarloResult(playerCards,
                    flopCards.Union(turnCards).Union(riverCards).ToList(),
                    countPlayers);
                var allCards = playerCards.Union(flopCards).Union(turnCards).Union(riverCards).ToArray();
                var layoutResolver = new LayoutResolver(new CardLayout(allCards));
                bestLayout = layoutResolver.PokerLayout;
                pokerPosition = position.GetPokerPosition(NumPlayers);
            }

            return new PokerResults(
                reconResults,
                matchResults,
                monteCarloResult,
                bestLayout,
                pokerPosition);
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
    }
}