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
        }

        public override void Show(Environment e)
        {
            var playerResult = GetResult(nameof(PlayerCards));
            var flopResult = GetResult(nameof(Flop));
            var turnResult = GetResult(nameof(Turn));
            var riverResult = GetResult(nameof(River));
            var positionResults = GetResultsPrefixed(nameof(Position)).ToList();
            var opponentResults = GetResultsPrefixed(nameof(Opponent)).ToList();

            var playerCards = _playerCards.Match(playerResult);
            var flopCards = _flop.Match(flopResult);
            var turnCards = _turn.Match(turnResult);
            var riverCards = _river.Match(riverResult);
            var position = _position.Match(positionResults);
            var opponents = _opponent.Match(opponentResults);

            _playerCards.GetPresenter().Present(playerCards, playerResult, e);
            _flop.GetPresenter().Present(flopCards, flopResult, e);
            _turn.GetPresenter().Present(turnCards, turnResult, e);
            _river.GetPresenter().Present(riverCards, riverResult, e);

            var positionPresenter = _position.GetPresenter();
            foreach (var positionResult in positionResults)
            {
                positionPresenter.Present(position, positionResult, e);
            }

            var opponentPresenter = _opponent.GetPresenter();
            foreach (var opponentResult in opponentResults)
            {
                opponentPresenter.Present(opponents, opponentResult, e);
            }

            if (playerCards.Any())
            {
                int countPlayers = opponents.Count + 1;
                var result = ComputeMonteCarloResult(playerCards, flopCards.Union(turnCards).Union(riverCards).ToList(),
                    countPlayers);

                var playerCardsStr = string.Join(" ", playerCards.Select(c => c.ToString()));
                var flopCardsStr = string.Join(" ", flopCards.Select(c => c.ToString())); 
                var turnCardsStr = string.Join(" ", turnCards.Select(c => c.ToString()));
                var riverCardsStr = string.Join(" ", riverCards.Select(c => c.ToString()));

                var font = new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular);
                var x = 10;
                var y = 10;

                void DrawColoredString(string text, Color color, ref int x, int y, Graphics g, Font font)
                {
                    g.DrawString(text, font, new SolidBrush(color), x, y);
                    x += (int)g.MeasureString(text + " ", font).Width;
                }

                var prefix = $"Players = {countPlayers} Layout = ";
                DrawColoredString(prefix, Color.Black, ref x, y, e.Graphics, font);
                DrawColoredString(playerCardsStr, Color.Black, ref x, y, e.Graphics, font);
                DrawColoredString(flopCardsStr, Color.FromArgb(205, 127, 50), ref x, y, e.Graphics, font); // Bronze
                DrawColoredString(turnCardsStr, Color.Orange, ref x, y, e.Graphics, font);
                DrawColoredString(riverCardsStr, Color.Violet, ref x, y, e.Graphics, font);

                e.Graphics.DrawString($": B {result.Better * 100:F1}% - E {result.Exact * 100:F1}% - S {result.Smaller * 100:F1}%",
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

            return spec;
        }

        private static MonteCarloResult ComputeMonteCarloResult(List<Card> myCards, List<Card> boardCards,
            int numOfPlayers)
        {
            CardSet cardSet = new CardSet();
            RandomSetDefinition arg = new RandomSetDefinition
            {
                MyLayout = new CardLayout(myCards.ToArray()),
                NumOfPlayers = numOfPlayers,
                Board = boardCards.ToArray()
            };
            MonteCarlo<CardSet, RandomSetDefinition> monteCarlo =
                new MonteCarlo<CardSet, RandomSetDefinition>(cardSet, 1000, arg);

            MonteCarloResult result = monteCarlo.Solve();
            return result;
        }
    }
}