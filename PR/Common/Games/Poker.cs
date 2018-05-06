using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.RegionMatchers;
using PT.Algorithm;
using PT.Algorithm.Model;
using PT.Poker.Model;
using PT.Poker.Resolving;

namespace Common.Games
{
    public class Poker : GameBase
    {
        private Flop _flop;
        private Turn _turn;
        private River _river;
        private PlayerCards _playerCards;

        public Poker(Project prj, Board board, Action<string> processor) : base(prj, board, processor)
        {
            InitializeMatchers();
        }

        private void InitializeMatchers()
        {
            _flop = new Flop(Board);
            _turn = new Turn(Board);
            _river = new River(Board);
            _playerCards = new PlayerCards(Board);
        }

        public override void Show(Environment e)
        {
            var playerResult = GetResult(nameof(PlayerCards));
            var flopResult = GetResult(nameof(Flop));
            var turnResult = GetResult(nameof(Turn));
            var riverResult = GetResult(nameof(River));

            var playerCards = _playerCards.Match(playerResult);
            var flopCards = _flop.Match(flopResult);
            var turnCards = _turn.Match(turnResult);
            var riverCards = _river.Match(riverResult);

            _playerCards.GetPresenter().Present(playerCards, playerResult, e);
            _flop.GetPresenter().Present(flopCards, flopResult, e);
            _turn.GetPresenter().Present(turnCards, turnResult, e);
            _river.GetPresenter().Present(riverCards, riverResult, e);

            //if (playerCards.Any())
            {
                var result = ComputeMonteCarloResult(playerCards, flopCards.Union(turnCards).Union(riverCards).ToList(),
                    2);

                var playerCardLayout =
                    new CardLayout(playerCards
                        .Union(flopCards).Union(turnCards).Union(riverCards));
                e.Graphics.DrawString(
                    $"{playerCardLayout}: {result.Better * 100}% - {result.Exact * 100}% - {result.Smaller * 100}%"
                    , new Font(FontFamily.GenericMonospace, 11, FontStyle.Regular), new SolidBrush(Color.Black), 10,
                    10);
            }

            ConsumeResult();
        }

        private GameResult GetResult(string name)
        {
            return GameResults[Board.Computed].First(x => x.Name == name);
        }

        protected override ImgReconSpec CreateImgReconSpec(Project project, Board board, string outFilePath)
        {
            ImgReconSpec spec = ImgReconSpec.CreateImgReconSpec(project, board, outFilePath);

            spec.RegionSpecs.Add(_flop.GetRegionSpec());
            spec.RegionSpecs.Add(_turn.GetRegionSpec());
            spec.RegionSpecs.Add(_river.GetRegionSpec());
            spec.RegionSpecs.Add(_playerCards.GetRegionSpec());
            return spec;
        }

        private void ConsumeResult()
        {
            GameResults.Remove(Board.Computed);
            Board.Consume();
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