using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.RegionMatchers;

namespace Common.Games
{
    public class Poker : GameBase
    {
        private const string SpecDir = "specs";
        private readonly Project _prj;
        private readonly Action<string> _processor;
        private Flop _flop;
        
        public Dictionary<int, List<GameResult>> GameResults=new Dictionary<int, List<GameResult>>();

        public Poker(Project prj, Board board, Action<string> processor) : base(board)
        {
            _prj = prj;
            _processor = processor;
            InitializeMatchers();
            
            if (!Directory.Exists(SpecDir))
            {
                Directory.CreateDirectory(SpecDir);
            }
        }

        private void InitializeMatchers()
        {
            _flop = new Flop(_board);
        }

        public void Process()
        {
            var outFilePath = $"{SpecDir}/out{_board.Generated}.txt";
            ImgReconSpec spec = new ImgReconSpec
            {
                //ImgPath = SaveLoad.GetBoardPath(_prj, _board), //todo iter
                ImgPath = SaveLoad.GetBoardPathIter(_prj, _board),
                RegionPath = RegionLoader.GetRegionPath("", _board),
                OutFilePath = outFilePath,
                RegionSpecs = new List<ImgReconSpec.RegionSpec>()
            };

            var cardsClassPath = Path.Combine($"{_board.Rect.Width}X{_board.Rect.Height}", "classes", "cards");

            spec.RegionSpecs.Add(new ImgReconSpec.RegionSpec
            {
                ClassesPath = cardsClassPath,
                Name = "flop",
                Num = 3,
                Threshold = -1
            });

            var specFileName = $"{SpecDir}/spec{_board.Generated}.txt";
            spec.Save(specFileName);

            _processor(specFileName);

            var sleepTime = 100;
            int iter = 10000 / sleepTime;
            int generated = _board.Generated;
            Task.Run(() =>
            {
                while (!File.Exists(outFilePath) && --iter > 0)
                {
                    Thread.Sleep(sleepTime);
                }

                if (File.Exists(outFilePath))
                {
                    var gameResults = new List<GameResult>();
                    var output = ImgReconOutput.Load(outFilePath);
                    int i = 0;
                    foreach (var specResult in output.SpecResults)
                    {
                        gameResults.Add(new GameResult(RegionLoader.LoadRegion("", _board, spec.RegionSpecs[i].Name),
                            string.Join(",", specResult.Values)));
                    }
                    GameResults[generated] = gameResults;
                }
            });
        }


        protected override void Analize()
        {
            var flopMatch = _flop.Match();
            if (flopMatch == "As")
            {
            }
        }
    }

    public class GameResult
    {
        public GameResult(Rectangle resultRect, string resultText)
        {
            ResultRect = resultRect;
            ResultText = resultText;
        }

        public Rectangle ResultRect { get; }
        public string ResultText { get; }
    }
}