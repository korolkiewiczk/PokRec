using System.Drawing;
using Common;
using Game.Common;

namespace Game.Presentation
{
    public class DecisionPresenter : IResultPresenter
    {
        public void Present(ReconResult reconResult, GameEnvironment e)
        {
            if (reconResult != null && reconResult.Results.Any())
            {
                var rect = Util.MapRect(reconResult.ItemRectangle, e);
                e.Graphics.DrawEllipse(new Pen(Color.Red), rect); // Using red color to distinguish decision indicator
            }
        }
    }
} 