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
            _position = new Position(Board, 9);
        }

        public override void Show(Environment e)
        {
            var playerResult = GetResult(nameof(PlayerCards));
            var flopResult = GetResult(nameof(Flop));
            var turnResult = GetResult(nameof(Turn));
            var riverResult = GetResult(nameof(River));
            var positionResults = GetResultsPrefixed(nameof(Position)).ToList();

            var playerCards = _playerCards.Match(playerResult);
            var flopCards = _flop.Match(flopResult);
            var turnCards = _turn.Match(turnResult);
            var riverCards = _river.Match(riverResult);
            var position = _position.Match(positionResults);

            _playerCards.GetPresenter().Present(playerCards, playerResult, e);
            _flop.GetPresenter().Present(flopCards, flopResult, e);
            _turn.GetPresenter().Present(turnCards, turnResult, e);
            _river.GetPresenter().Present(riverCards, riverResult, e);

            var positionPresenter = _position.GetPresenter();
            foreach (var positionResult in positionResults)
            {
                positionPresenter.Present(position, positionResult, e);
            }

            if (playerCards.Any())
            {
                var result = ComputeMonteCarloResult(playerCards, flopCards.Union(turnCards).Union(riverCards).ToList(),
                    2);

                var playerCardLayout =
                    new CardLayout(playerCards
                        .Union(flopCards).Union(turnCards).Union(riverCards));
                e.Graphics.DrawString(
                    $"{position + 1} {playerCardLayout}: {result.Better * 100}% - {result.Exact * 100}% - {result.Smaller * 100}%"
                    , new Font(FontFamily.GenericMonospace, 11, FontStyle.Regular), new SolidBrush(Color.Black), 10,
                    10);
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