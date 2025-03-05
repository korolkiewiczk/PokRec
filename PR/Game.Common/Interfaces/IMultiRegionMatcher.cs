using System.Collections.Generic;
using Common;
using Game.Common;

namespace Game.Interfaces
{
    public interface IMultiRegionMatcher<out T>
    {
        T Match(IEnumerable<ReconResult> results);
        IEnumerable<RegionSpec> GetRegionSpecs();
    }
}