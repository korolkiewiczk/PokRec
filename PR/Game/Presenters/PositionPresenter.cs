using System.Drawing;
using System.Linq;

namespace Game.Presenters
{
    public class PositionPresenter : IResultPresenter<int>
    {
        public void Present(int result, ReconResult reconResult, Environment e)
        {
            if (reconResult.Results.Any())
            {
                var rect = Util.MapRect(reconResult.ItemRectangle, e);
                e.Graphics.DrawEllipse(new Pen(Color.Black), rect);
            }
        }
    }
}