using System.Collections.Generic;
using Common;
using Game.Presenters;

namespace Game.MultiRegionMatchers
{
    public class Position : MultiPosMatcher
    {
        public Position(Board board, int seats) : base(board, seats)
        {
        }

        public override IResultPresenter<Place> GetPresenter()
        {
            return new PositionPresenter();
        }

        public override IEnumerable<RegionSpec> GetRegionSpecs()
        {
            List<RegionSpec> specs = new List<RegionSpec>();
            for (int i = 0; i < Seats; i++)
            {
                specs.Add(new RegionSpec
                {
                    ClassesPath = Classes.ClassPath(Board, "position"),
                    Name = $"{nameof(Position)}{i + 1}",
                    Num = 1,
                    Threshold = 80,
                    AbandonThreshold = 50
                });
            }

            return specs;
        }
    }
}