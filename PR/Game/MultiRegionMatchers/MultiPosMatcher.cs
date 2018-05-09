using System.Collections.Generic;
using System.Linq;
using Common;

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

        public abstract IResultPresenter<Place> GetPresenter();

        public abstract IEnumerable<RegionSpec> GetRegionSpecs();
    }
}