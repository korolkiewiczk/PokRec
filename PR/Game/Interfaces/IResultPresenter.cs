using Common;
using Game.Common;

namespace Game.Interfaces
{
    public interface IResultPresenter
    {
        void Present(ReconResult reconResult, Environment e);
    }
}