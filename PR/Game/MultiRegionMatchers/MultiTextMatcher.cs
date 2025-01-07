using System.Collections.Generic;
using Common;

namespace Game.MultiRegionMatchers
{
    public abstract class MultiTextMatcher<T> : TextMatcher<List<T>>
    {
        private readonly int _seats;

        protected MultiTextMatcher(int seats)
        {
            _seats = seats;
        }

        public override IEnumerable<RegionSpec> GetRegionSpecs()
        {
            List<RegionSpec> specs = new List<RegionSpec>();
            for (int i = 0; i < _seats; i++)
            {
                var spec = GetSingleRegionSpec(i);
                specs.Add(spec);
            }

            return specs;
        }
    }
} 