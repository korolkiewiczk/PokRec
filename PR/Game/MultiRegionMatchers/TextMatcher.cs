using System.Collections.Generic;
using Common;
using Game.Interfaces;

namespace Game.MultiRegionMatchers
{
    public abstract class TextMatcher<T> : IMultiRegionMatcher<T>
    {
        protected RegionSpec GetSingleRegionSpec(int i, bool isNumericOnly)
        {
            return new RegionSpec
            {
                Name = i == -1 ? GetType().Name : $"{GetType().Name}{i + 1}",
                Num = 1,
                IsOcr = true,
                IsNumericOnly = isNumericOnly
            };
        }

        public abstract T Match(IEnumerable<ReconResult> results);

        public abstract IResultPresenter GetPresenter();

        public abstract IEnumerable<RegionSpec> GetRegionSpecs();
    }
}