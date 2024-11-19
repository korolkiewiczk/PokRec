using System.Collections.Generic;
using Common;
using Game.Presenters;

namespace Game.MultiRegionMatchers
{
    public class Opponent : MultiPosMatcher
    {
        public Opponent(Board board, int seats) : base(board, seats)
        {
        }

        public override IResultPresenter<Place> GetPresenter()
        {
            return new OpponentPresenter();
        }

        public override IEnumerable<RegionSpec> GetRegionSpecs()
        {
            List<RegionSpec> specs = new List<RegionSpec>();
            for (int i = 0; i < Seats; i++)
            {
                specs.Add(new RegionSpec
                {
                    ClassesPath = Classes.ClassPath(Board, "opponent"),
                    Name = $"{nameof(Opponent)}{i + 1}",
                    Num = 1,
                    Threshold = 70,
                    AbandonThreshold = 50
                });
            }

            return specs;
        }
    }
}