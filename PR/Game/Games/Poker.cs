using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Common;
using Game.MultiRegionMatchers;
using Game.RegionMatchers;
using PT.Algorithm;
using PT.Algorithm.Model;
using PT.Poker.Model;
using PT.Poker.Resolving;

namespace Game.Games
{
    public class Poker : GameBase
    {
        private Flop _flop;
        private Turn _turn;
        private River _river;
        private PlayerCards _playerCards;
        private Position _position;
        private Opponent _opponent;
        private Stack _stack;
        private int _numPlayers;

        public Poker(Project project, Board board, Action<string> processor) : base(project, board, processor)
        {
            InitializeMatchers();
        }

        private void InitializeMatchers()
        {
            _flop = new Flop(Board);
            _turn = new Turn(Board);
            _river = new River(Board);
            _playerCards = new PlayerCards(Board);

            var settings = new PokerBoardSettingsParser(Board);
            _position = new Position(Board, settings.Players);
            _opponent = new Opponent(Board, settings.Players - 1);
            _stack = new Stack(settings.Players);
            _numPlayers = settings.Players;
        }

        public override void Show(Environment e)
        {
            var playerResult = GetResult(nameof(PlayerCards));
            var flopResult = GetResult(nameof(Flop));
            var turnResult = GetResult(nameof(Turn));
            var riverResult = GetResult(nameof(River));
            var positionResults = GetResultsPrefixed(nameof(Position)).ToList();
            var opponentResults = GetResultsPrefixed(nameof(Opponent)).ToList();
            var stackResults = GetResultsPrefixed(nameof(Stack)).ToList();
            var playerCards = _playerCards.Match(playerResult);
            var flopCards = _flop.Match(flopResult);
            var turnCards = _turn.Match(turnResult);
            var riverCards = _river.Match(riverResult);
            var position = _position.Match(positionResults);
            var opponents = _opponent.Match(opponentResults);
            var stack = _stack.Match(stackResults);
            _playerCards.GetPresenter().Present(playerResult, e);
            _flop.GetPresenter().Present(flopResult, e);
            _turn.GetPresenter().Present(turnResult, e);
            _river.GetPresenter().Present(riverResult, e);

            var positionPresenter = _position.GetPresenter();
            foreach (var positionResult in positionResults)
            {
                positionPresenter.Present(positionResult, e);
            }

            var opponentPresenter = _opponent.GetPresenter();
            foreach (var opponentResult in opponentResults)
            {
                opponentPresenter.Present(opponentResult, e);
            }

            var stackPresenter = _stack.GetPresenter();
            foreach (var stackResult in stackResults)
            {
                stackPresenter.Present(stackResult, e);
            }

            if (playerCards.Any())
            {
                int countPlayers = opponents.Count + 1; // +1 for the player
                var result = ComputeMonteCarloResult(playerCards, flopCards.Union(turnCards).Union(riverCards).ToList(),
                    countPlayers);
                
                

                var playerCardsStr = string.Join(" ", playerCards.Select(c => c.ToString()));
                var flopCardsStr = string.Join(" ", flopCards.Select(c => c.ToString())); 
                var turnCardsStr = string.Join(" ", turnCards.Select(c => c.ToString()));
                var riverCardsStr = string.Join(" ", riverCards.Select(c => c.ToString()));

                void DrawColoredString(string text, Color color, ref int x, int y, Graphics g, Font font)
                {
                    g.DrawString(text, font, new SolidBrush(color), x, y);
                    x += (int)g.MeasureString(text + " ", font).Width;
                }
                
                var font = new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular);
                var lineHeight = font.GetHeight(e.Graphics);
                // First line - Players count and Position
                var x = 10;
                var y = 10;

                var positionText = position.GetPokerPosition(_numPlayers).ToDisplayString();
                DrawColoredString($"Players = {countPlayers} Position = {positionText}", Color.Black, ref x, y, e.Graphics, font);
                // Second line - Layout and cards
                x = 10;
                y += (int)lineHeight;
                const string prefix2 = "Layout = ";
                DrawColoredString(prefix2, Color.Black, ref x, y, e.Graphics, font);
                DrawColoredString(playerCardsStr, Color.Black, ref x, y, e.Graphics, font);
                DrawColoredString(flopCardsStr, Color.FromArgb(205, 127, 50), ref x, y, e.Graphics, font); // Bronze
                DrawColoredString(turnCardsStr, Color.Orange, ref x, y, e.Graphics, font);
                DrawColoredString(riverCardsStr, Color.Violet, ref x, y, e.Graphics, font);

                // Determine the best layout
                var allCards = playerCards.Union(flopCards).Union(turnCards).Union(riverCards).ToArray();
                var layoutResolver = new LayoutResolver(new CardLayout(allCards));
                var bestLayout = layoutResolver.PokerLayout.ToDisplayString();

                // Display the best layout
                x = 10;
                y += (int)lineHeight;
                DrawColoredString($"Best Poker Hand: {bestLayout}", layoutResolver.PokerLayout.ToColor(), ref x, y, e.Graphics, font);
                
                // Second line
                x = 10;
                y += (int)lineHeight;
                e.Graphics.DrawString($"WIN {result.Better * 100:F1}% - TIE {result.Exact * 100:F1}% - LOSE {result.Smaller * 100:F1}%",
                    font, new SolidBrush(Color.Black), x, y);
                // Second line
                x = 10;
                y += (int)lineHeight;
                e.Graphics.DrawString(string.Join(',', stack.Select((x,i)=>$"Player {i+1} has ${x}")),
                    font, new SolidBrush(Color.Black), x, y);
            }

            ConsumeResult();
        }

        protected override ImgReconSpec CreateImgReconSpec(string outFilePath)
        {
            ImgReconSpec spec = ImgReconSpec.CreateImgReconSpec(Project, Board, outFilePath, PrevOutputFilePath);

            spec.RegionSpecs.Add(_flop.GetRegionSpec());
            spec.RegionSpecs.Add(_turn.GetRegionSpec());
            spec.RegionSpecs.Add(_river.GetRegionSpec());
            spec.RegionSpecs.Add(_playerCards.GetRegionSpec());

            spec.RegionSpecs.AddRange(_position.GetRegionSpecs());
            spec.RegionSpecs.AddRange(_opponent.GetRegionSpecs());
            spec.RegionSpecs.AddRange(_stack.GetRegionSpecs());

            return spec;
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
                new MonteCarlo<CardSet, RandomSetDefinition>(1000, arg);

            MonteCarloResult result = monteCarlo.Solve();
            return result;
        }
    }
}