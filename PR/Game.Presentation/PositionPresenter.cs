using System.Drawing;
using Common;
using Game.Common;

namespace Game.Presentation
{
    public class PositionPresenter : IResultPresenter
    {
        public void Present(ReconResult reconResult, GameEnvironment e)
        {
            if (reconResult.Results.Any())
            {
                var rect = Util.MapRect(reconResult.ItemRectangle, e);
                e.Graphics.DrawEllipse(new Pen(Color.Black), rect);
            }
        }
    }
}