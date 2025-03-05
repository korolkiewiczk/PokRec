using Common;

namespace Game.Presentation
{
    public interface IResultPresenter
    {
        void Present(ReconResult reconResult, GameEnvironment e);
    }
}