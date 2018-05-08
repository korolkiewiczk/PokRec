using System.Collections.Generic;
using Common;

namespace Game
{
    public interface IMultiRegionMatcher<T>
    {
        T Match(IEnumerable<ReconResult> results);
        IResultPresenter<T> GetPresenter();
        IEnumerable<RegionSpec> GetRegionSpecs();
    }
}