using System.Drawing;
using System.Linq;
using Game.MultiRegionMatchers;

namespace Game.Presenters
{
    public class OpponentPresenter : IResultPresenter<Place>
    {
        public void Present(Place result, ReconResult reconResult, Environment e)
        {
            if (reconResult.Results.Any())
            {
                var rect = Util.MapRect(reconResult.ItemRectangle, e);
                e.Graphics.DrawRectangle(new Pen(Color.Black), rect);
            }
        }
    }
} 