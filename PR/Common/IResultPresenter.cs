using System.Drawing;

namespace Common
{
    public interface IResultPresenter
    {
        void Present(GameResult result, Environment e);
    }
}