using System;
using System.Collections.Generic;
using System.Drawing;
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
        private readonly Action<string> _processor;
        private Flop _flop;

        public readonly Dictionary<int, List<GameResult>> GameResults = new Dictionary<int, List<GameResult>>();

        public Poker(Project prj, Board board, Action<string> processor) : base(prj, board)
        {
            _processor = processor;

            InitializeMatchers();
        }

        private void InitializeMatchers()
        {
            _flop = new Flop(_board);
        }

        public void Process()
        {
            var outFilePath = ImgReconOutput.OutFilePath(_prj, _board);

            var spec = CreateImgReconSpec(_prj, _board, outFilePath);

            ProcessSpec(spec);

            Task.Run(() =>
            {
                WaitForFile(outFilePath);

                if (File.Exists(outFilePath))
                {
                    GameResults[_board.Generated] = CollectResults(outFilePath, _board, spec);
                }
            });
        }

        public void ShowMatch(int boardNum, Environment e)
        {
            var gameResults = GameResults[boardNum];
            var flopCards = _flop.Match(gameResults.First(x => x.Name == nameof(Flop)));
            var playerCardLayout =
                new CardLayout(new List<Card>
                {
                    new Card(CardColor.Clubs, CardType.A),
                    new Card(CardColor.Diamonds, CardType.A)
                }.Union(flopCards));

            var result = ComputeMonteCarloResult(playerCardLayout.Cards.ToList(), 2);
            e.Graphics.DrawString(string.Format("{0}% - {1}% - {2}%", result.Better * 100, result.Exact * 100,
                    result.Smaller * 100)
                , new Font(FontFamily.GenericMonospace, 11, FontStyle.Regular), new SolidBrush(Color.Black), 10, 10);
        }

        private static MonteCarloResult ComputeMonteCarloResult(List<Card> cards, int numOfPlayers)
        {
            CardSet cardSet = new CardSet();
            RandomSetDefinition arg = new RandomSetDefinition
            {
                MyLayout = new CardLayout(cards.Take(2).ToArray()),
                // ReSharper disable once PossibleInvalidOperationException
                NumOfPlayers = numOfPlayers,
                Board = cards.Skip(2).Take(5).ToArray()
            };
            MonteCarlo<CardSet, RandomSetDefinition> monteCarlo =
                new MonteCarlo<CardSet, RandomSetDefinition>(cardSet, 10000, arg);

            MonteCarloResult result = monteCarlo.Solve();
            return result;
        }

        private void ProcessSpec(ImgReconSpec spec)
        {
            var specFileName = ImgReconSpec.SpecFileName(_prj, _board);
            spec.Save(specFileName);
            _processor(specFileName);
        }

        private ImgReconSpec CreateImgReconSpec(Project project, Board board, string outFilePath)
        {
            ImgReconSpec spec = ImgReconSpec.CreateImgReconSpec(project, board, outFilePath);

            spec.RegionSpecs.Add(_flop.GetRegionSpec());
            return spec;
        }
    }
}