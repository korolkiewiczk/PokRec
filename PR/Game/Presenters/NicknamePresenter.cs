using System.Drawing;
using System.Linq;
using Common;
using Game.Common;
using Game.Interfaces;

namespace Game.Presenters
{
    public class NicknamePresenter : IResultPresenter
    {
        public void Present(ReconResult reconResult, Environment e)
        {
            if (reconResult.Results.Any())
            {
                var rect = Util.MapRect(reconResult.ItemRectangle, e);
                
                // Draw rectangle around the nickname
                e.Graphics.DrawRectangle(new Pen(Color.Blue), rect);

                // Draw the nickname text
                e.Graphics.DrawString(reconResult.Results.FirstOrDefault(),
                    new Font(FontFamily.GenericMonospace, 8),
                    Brushes.Blue,
                    rect.Location);
            }
        }
    }
} 