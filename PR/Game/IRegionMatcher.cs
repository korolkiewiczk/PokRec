using Common;

namespace Game
{
    public interface IRegionMatcher<T>
    {
        RegionSpec GetRegionSpec();
        T Match(ReconResult result);
        IResultPresenter<T> GetPresenter();
    }
}
