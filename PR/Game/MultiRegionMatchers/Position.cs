using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Game.Presenters;

namespace Game.MultiRegionMatchers
{
    public class Position : IMultiRegionMatcher<int>
    {
        private Board _board;
        private readonly int _seats;

        public Position(Board board, int seats)
        {
            _board = board;
            _seats = seats;
        }

        public int Match(IEnumerable<ReconResult> results)
        {
            return results.Select((x, i) => new {x, i}).Where(x => x.x.Results.Any()).FirstOrDefault()?.i ?? -1;
        }

        public IResultPresenter<int> GetPresenter()
        {
            return new PositionPresenter();
        }

        public IEnumerable<RegionSpec> GetRegionSpecs()
        {
            List<RegionSpec> specs = new List<RegionSpec>();
            for (int i = 0; i < _seats; i++)
            {
                specs.Add(new RegionSpec
                {
                    ClassesPath = Classes.ClassPath(_board, "position"),
                    Name = $"{nameof(Position)}{i + 1}",
                    Num = 1,
                    Threshold = 95,
                    AbandonThreshold = 70
                });
            }

            return specs;
        }
    }
}