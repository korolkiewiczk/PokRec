using System.Drawing;
using System.Linq;
using Common;
using Game.Common;
using Game.Interfaces;

namespace Game.Presenters
{
    public class PositionPresenter : IResultPresenter
    {
        public void Present(ReconResult reconResult, Environment e)
        {
            if (reconResult.Results.Any())
            {
                var rect = Util.MapRect(reconResult.ItemRectangle, e);
                e.Graphics.DrawEllipse(new Pen(Color.Black), rect);
            }
        }
    }
}