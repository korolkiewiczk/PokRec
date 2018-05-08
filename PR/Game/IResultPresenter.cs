namespace Game
{
    public interface IResultPresenter<T>
    {
        void Present(T result, ReconResult reconResult, Environment e);
    }
}