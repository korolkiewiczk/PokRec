using System.Collections.Generic;
using Common;

namespace Game.Interfaces
{
    public interface IMultiRegionMatcher<out T>
    {
        T Match(IEnumerable<ReconResult> results);
        IResultPresenter GetPresenter();
        IEnumerable<RegionSpec> GetRegionSpecs();
    }
}