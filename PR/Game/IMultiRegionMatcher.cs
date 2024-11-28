using System.Collections.Generic;
using Common;

namespace Game
{
    public interface IMultiRegionMatcher<T>
    {
        T Match(IEnumerable<ReconResult> results);
        IResultPresenter GetPresenter();
        IEnumerable<RegionSpec> GetRegionSpecs();
    }
}