using Common;
using Game.Common;

namespace Game.Interfaces
{
    public interface IRegionMatcher<out T>
    {
        RegionSpec GetRegionSpec();
        T Match(ReconResult result);
    }
}
