using System.Drawing;
using Common;
using Game.Common;
using Game.Common.Utils;

namespace Game.Presentation
{
    public class StackPresenter : IResultPresenter
    {
        public void Present(ReconResult reconResult, GameEnvironment e)
        {
            if (reconResult != null && reconResult.Results.Any())
            {
                var rect = Util.MapRect(reconResult.ItemRectangle, e);

                // Draw rectangle around the stack value
                e.Graphics.DrawRectangle(new Pen(Color.Green), rect);

                // Draw the stack value
                e.Graphics.DrawString($"${MoneyParser.ParseMoneyValue(reconResult.Results.FirstOrDefault())}",
                    new Font(FontFamily.GenericMonospace, 8),
                    Brushes.Green,
                    rect.Location);
            }
        }
    }
}