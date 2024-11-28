using System.Drawing;

namespace Game.Presenters
{
    public class CardPresenter : IResultPresenter
    {
        public void Present(ReconResult reconResult, Environment e)
        {
            var rect = Util.MapRect(reconResult.ItemRectangle, e);

            e.Graphics.DrawRectangle(new Pen(Color.Black), rect);
            e.Graphics.DrawString(string.Join(",", reconResult.Results),
                new Font(FontFamily.GenericMonospace, 8), Brushes.Black,
                rect.X, rect.Y);
        }
    }
}