using Common;

namespace Game.Interfaces
{
    public interface IRegionMatcher<out T>
    {
        RegionSpec GetRegionSpec();
        T Match(ReconResult result);
        IResultPresenter GetPresenter();
    }
}
