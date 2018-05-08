﻿using System;
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
        protected readonly Dictionary<int, List<GameResult>> GameResults = new Dictionary<int, List<GameResult>>();
        protected string PrevOutputFilePath;

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

            if (File.Exists(outFilePath))
            {
                File.Delete(outFilePath);
            }

            var spec = CreateImgReconSpec(outFilePath);

            ProcessSpec(spec);

            Task.Run(() =>
            {
                WaitForFile(outFilePath);

                if (File.Exists(outFilePath))
                {
                    CleanupSpecs();

                    GameResults[Board.Computed] = CollectResults(outFilePath, Board, spec);
                    PrevOutputFilePath = outFilePath;
                }
            });
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
            var specFileName = ImgReconSpec.SpecFileName(Project, Board);
            spec.Save(specFileName);
            _processor(specFileName);
        }

        protected void ConsumeResult()
        {
            GameResults.Clear();
            Board.Consume();
        }

        protected GameResult GetResult(string name)
        {
            return GameResults[Board.Computed].First(x => x.Name == name);
        }
    }
}