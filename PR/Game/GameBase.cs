using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace Game
{
    public abstract class GameBase
    {
        protected readonly Board Board;
        private readonly Project _prj;
        private readonly Action<string> _processor;
        protected readonly Dictionary<int, List<GameResult>> GameResults = new Dictionary<int, List<GameResult>>();

        protected GameBase(Project prj, Board board, Action<string> processor)
        {
            _prj = prj;
            Board = board;
            _processor = processor;

            if (!Directory.Exists(ImgReconSpec.SpecDirectory(_prj, Board)))
            {
                Directory.CreateDirectory(ImgReconSpec.SpecDirectory(_prj, Board));
            }
            else
            {
                foreach (var file in Directory.GetFiles(ImgReconSpec.SpecDirectory(_prj, Board),"spec*"))
                {
                    File.Delete(file);
                }
            }
        }

        public void Process()
        {
            if (File.Exists(ImgReconSpec.SpecFileName(_prj, Board)))
            {
                return;
            }

            var outFilePath = ImgReconOutput.OutFilePath(_prj, Board);

            if (File.Exists(outFilePath))
            {
                File.Delete(outFilePath);
            }

            var spec = CreateImgReconSpec(_prj, Board, outFilePath);

            ProcessSpec(spec);

            Task.Run(() =>
            {
                WaitForFile(outFilePath);

                if (File.Exists(outFilePath))
                {
                    File.Delete(ImgReconSpec.SpecFileName(_prj, Board));
                    GameResults[Board.Computed] = CollectResults(outFilePath, Board, spec);
                }
            });
        }

        public abstract void Show(Environment e);

        public bool HasComputed()
        {
            return GameResults.ContainsKey(Board.Computed);
        }

        protected abstract ImgReconSpec CreateImgReconSpec(Project project, Board board, string outFilePath);

        private static void WaitForFile(string filePath)
        {
            var sleepTime = 100;
            int iter = 10000 / sleepTime;    //todo config
            while (!File.Exists(filePath) && --iter > 0)
            {
                Thread.Sleep(sleepTime);
            }
        }

        private static List<GameResult> CollectResults(string outFilePath, Board board, ImgReconSpec spec)
        {
            var gameResults = new List<GameResult>();
            var output = ImgReconOutput.Load(outFilePath);
            int i = 0;
            foreach (var specResult in output.SpecResults)
            {
                gameResults.Add(new GameResult(
                    specResult.Name, 
                    RegionLoader.LoadRegion(board, spec.RegionSpecs[i].Name),
                    specResult.Values)
                );    
                i++;
            }

            return gameResults;
        }
        
        private void ProcessSpec(ImgReconSpec spec)
        {
            var specFileName = ImgReconSpec.SpecFileName(_prj, Board);
            spec.Save(specFileName);
            _processor(specFileName);
        }
    }
}