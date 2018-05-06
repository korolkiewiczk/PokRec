namespace Common
{
    public interface IRegionMatcher<T>
    {
        RegionSpec GetRegionSpec();
        T Match(GameResult result);
        IResultPresenter<T> GetPresenter();
    }
}
