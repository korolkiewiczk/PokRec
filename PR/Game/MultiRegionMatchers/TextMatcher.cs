using System.Collections.Generic;
using Common;
using Game.Presenters;

namespace Game.MultiRegionMatchers
{
    public abstract class TextMatcher<T> : IMultiRegionMatcher<T>
    {
        public TextMatcher()
        {
        }

        protected RegionSpec GetSingleRegionSpec(int i = -1)
        {
            return new RegionSpec
            {
                Name = i == -1 ? GetType().Name : $"{GetType().Name}{i + 1}",
                Num = 1,
                IsOcr = true
            };
        }

        public abstract T Match(IEnumerable<ReconResult> results);

        public abstract IResultPresenter GetPresenter();

        public abstract IEnumerable<RegionSpec> GetRegionSpecs();
    }
}