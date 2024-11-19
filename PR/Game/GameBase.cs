using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace Game
{
    public abstract class GameBase
    {
        protected readonly Board Board;
        protected readonly Project Project;
        private readonly Action<string> _processor;
        protected readonly Dictionary<int, List<ReconResult>> GameResults = new Dictionary<int, List<ReconResult>>();
        protected string PrevOutputFilePath;

        protected readonly int sleepTime = 50;

        protected GameBase(Project project, Board board, Action<string> processor)
        {
            Project = project;
            Board = board;
            _processor = processor;

            if (!Directory.Exists(ImgReconSpec.SpecDirectory(Project, Board)))
            {
                Directory.CreateDirectory(ImgReconSpec.SpecDirectory(Project, Board));
            }
            else
            {
                CleanupSpecs();
            }
        }

        public void Process()
        {
            if (File.Exists(ImgReconSpec.SpecFileName(Project, Board)))
            {
                return;
            }

            var outFilePath = ImgReconOutput.OutFilePath(Project, Board);

            DeleteFileIfExists(outFilePath);

            var spec = CreateImgReconSpec(outFilePath);

            ProcessSpec(spec);

            Task.Run(() =>
            {
                WaitForFile(outFilePath);

                if (File.Exists(outFilePath))
                {
                    CleanupSpecs();

                    GameResults[Board.Computed] = CollectResults(Project, outFilePath, Board, spec);
                    PrevOutputFilePath = outFilePath;
                }
            });
        }

        private void DeleteFileIfExists(string outFilePath)
        {
            if (File.Exists(outFilePath))
            {
                try
                {
                    File.Delete(outFilePath);
                }
                catch
                {
                    Thread.Sleep(sleepTime);
                    File.Delete(outFilePath);
                }
            }
        }

        private void CleanupSpecs()
        {
            foreach (var specFile in Directory.GetFiles(ImgReconSpec.SpecDirectory(Project, Board), "spec*"))
            {
                File.Delete(specFile);
            }
        }

        public abstract void Show(Environment e);

        public bool HasComputed()
        {
            return GameResults.ContainsKey(Board.Computed);
        }

        protected abstract ImgReconSpec CreateImgReconSpec(string outFilePath);

        private void WaitForFile(string filePath)
        {
            int iter = 10000 / sleepTime;
            while (!File.Exists(filePath) && --iter > 0)
            {
                Thread.Sleep(sleepTime);
            }
        }

        private static List<ReconResult> CollectResults(Project project, string outFilePath, Board board,
            ImgReconSpec spec)
        {
            var gameResults = new List<ReconResult>();
            var output = ImgReconOutput.Load(outFilePath);
            int i = 0;
            foreach (var specResult in output.SpecResults)
            {
                gameResults.Add(new ReconResult(
                    specResult.Name, 
                    RegionLoader.LoadRegion(project, board, spec.RegionSpecs[i].Name),
                    specResult.Values)
                );    
                i++;
            }

            return gameResults;
        }
        
        private void ProcessSpec(ImgReconSpec spec)
        {
            var specFileName = ImgReconSpec.SpecFileName(Project, Board);
            spec.Save(specFileName);
            _processor(specFileName);
        }

        protected void ConsumeResult()
        {
            GameResults.Clear();
            Board.Consume();
        }

        protected ReconResult GetResult(string name)
        {
            return GameResults[Board.Computed].First(x => x.Name == name);
        }
        
        protected IEnumerable<ReconResult> GetResultsPrefixed(string name)
        {
            return GameResults[Board.Computed].Where(x => x.Name.StartsWith(name));
        }
    }
}