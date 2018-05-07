namespace Game
{
    public interface IResultPresenter<T>
    {
        void Present(T result, GameResult gameResult, Environment e);
    }
}