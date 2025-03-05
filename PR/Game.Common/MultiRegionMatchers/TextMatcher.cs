using System.Collections.Generic;
using Common;
using Game.Interfaces;

namespace Game.Common.MultiRegionMatchers
{
    public abstract class TextMatcher<T> : IMultiRegionMatcher<T>
    {
        protected RegionSpec GetSingleRegionSpec(int i)
        {
            return new RegionSpec
            {
                Name = i == -1 ? GetType().Name : $"{GetType().Name}{i + 1}",
                Num = 1,
                IsOcr = true
            };
        }

        public abstract T Match(IEnumerable<ReconResult> results);

        public abstract IEnumerable<RegionSpec> GetRegionSpecs();
    }
}