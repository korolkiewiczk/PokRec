using Common;

namespace Game
{
    public interface IRegionMatcher<T>
    {
        RegionSpec GetRegionSpec();
        T Match(GameResult result);
        IResultPresenter<T> GetPresenter();
    }
}
