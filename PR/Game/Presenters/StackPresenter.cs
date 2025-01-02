using System.Drawing;
using System.Linq;
using Common;
using Game.Interfaces;
using Game.MultiRegionMatchers;
using Environment = Game.Common.Environment;

namespace Game.Presenters
{
    public class StackPresenter : IResultPresenter
    {
        public void Present(ReconResult reconResult, Environment e)
        {
            if (reconResult.Results.Any())
            {
                var rect = Util.MapRect(reconResult.ItemRectangle, e);

                // Draw rectangle around the stack value
                e.Graphics.DrawRectangle(new Pen(Color.Green), rect);

                // Draw the stack value
                e.Graphics.DrawString($"${Stack.ParseMoneyValue(reconResult.Results.FirstOrDefault())}",
                    new Font(FontFamily.GenericMonospace, 8),
                    Brushes.Green,
                    rect.Location);
            }
        }
    }
}