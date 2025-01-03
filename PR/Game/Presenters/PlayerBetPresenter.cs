using System.Drawing;
using System.Linq;
using Common;
using Game.Common;
using Game.Interfaces;

namespace Game.Presenters
{
    public class PlayerBetPresenter : IResultPresenter
    {
        public void Present(ReconResult reconResult, Environment e)
        {
            if (reconResult.Results.Any())
            {
                var rect = Util.MapRect(reconResult.ItemRectangle, e);
                
                // Draw rectangle around the bet amount
                e.Graphics.DrawRectangle(new Pen(Color.Green), rect);

                // Draw the bet amount text
                e.Graphics.DrawString(reconResult.Results.FirstOrDefault(),
                    new Font(FontFamily.GenericMonospace, 8),
                    Brushes.Green,
                    rect.Location);
            }
        }
    }
} 