using System.Drawing;
using System.Linq;
using Common;
using Game.Common;
using Game.Interfaces;

namespace Game.Presenters
{
    public class DecisionPresenter : IResultPresenter
    {
        public void Present(ReconResult reconResult, Environment e)
        {
            if (reconResult != null && reconResult.Results.Any())
            {
                var rect = Util.MapRect(reconResult.ItemRectangle, e);
                e.Graphics.DrawEllipse(new Pen(Color.Red), rect); // Using red color to distinguish decision indicator
            }
        }
    }
} 