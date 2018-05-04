using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public abstract class GameBase
    {
        protected readonly Board _board;
        protected readonly Project _prj;

        protected GameBase(Project prj, Board board)
        {
            _prj = prj;
            _board = board;

            if (!Directory.Exists(ImgReconSpec.SpecDirectory(_prj, _board)))
            {
                Directory.CreateDirectory(ImgReconSpec.SpecDirectory(_prj, _board));
            }
        }

        protected static void WaitForFile(string filePath)
        {
            var sleepTime = 100;
            int iter = 10000 / sleepTime;
            while (!File.Exists(filePath) && --iter > 0)
            {
                Thread.Sleep(sleepTime);
            }
        }

        protected static List<GameResult> CollectResults(string outFilePath, Board board, ImgReconSpec spec)
        {
            var gameResults = new List<GameResult>();
            var output = ImgReconOutput.Load(outFilePath);
            int i = 0;
            foreach (var specResult in output.SpecResults)
            {
                gameResults.Add(new GameResult(specResult.Name, RegionLoader.LoadRegion(board, spec.RegionSpecs[i].Name),
                    specResult.Values, spec.RegionSpecs[i].GetPresenter()));
                i++;
            }

            return gameResults;
        }
    }
}