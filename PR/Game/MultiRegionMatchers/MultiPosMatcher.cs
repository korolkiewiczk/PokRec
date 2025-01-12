using System.Collections.Generic;
using System.Linq;
using Common;
using Game.Interfaces;

namespace Game.MultiRegionMatchers
{
    public abstract class MultiPosMatcher : IMultiRegionMatcher<Place>
    {
        protected readonly Board Board;
        protected readonly int Seats;

        protected MultiPosMatcher(Board board, int seats)
        {
            Board = board;
            Seats = seats;
        }
        
        public Place Match(IEnumerable<ReconResult> results)
        {
            var listResults = results.ToList();
            Place place = new Place();
            for (int i = 0; i < listResults.Count; i++)
            {
                if (listResults[i].Results.Any())
                {
                    place.Add(i);
                }
            }

            return place;
        }

        public abstract IResultPresenter GetPresenter();

        public virtual IEnumerable<RegionSpec> GetRegionSpecs()
        {
            List<RegionSpec> specs = new List<RegionSpec>();
            for (int i = 0; i < Seats; i++)
            {
                specs.Add(new RegionSpec
                {
                    ClassesPath = Classes.ClassPath(Board, GetClassPathName()),
                    Name = $"{GetMatcherName()}{i + 1}",
                    Num = 1,
                    Threshold = GetThreshold()
                });
            }

            return specs;
        }

        protected abstract string GetClassPathName();
        protected abstract string GetMatcherName();
        protected abstract int GetThreshold();
    }
}