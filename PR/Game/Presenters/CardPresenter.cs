using System.Drawing;

namespace Game.Presenters
{
    public class CardPresenter : IResultPresenter
    {
        public void Present(ReconResult reconResult, Environment e)
        {
            var rect = Util.MapRect(reconResult.ItemRectangle, e);

            e.Graphics.DrawRectangle(new Pen(Color.Black), rect);
            var results = reconResult.Results;
            var font = new Font(FontFamily.GenericMonospace, 8);
            float x = rect.X;
            foreach (var result in results)
            {
                var brush = result[^1] switch
                {
                    's' => Brushes.Black,
                    'c' => Brushes.Green,
                    'h' => Brushes.Red,
                    'd' => Brushes.Blue,
                    _ => Brushes.Black
                };

                e.Graphics.DrawString(result, font, brush, x, rect.Y);
                x += (int) e.Graphics.MeasureString(result, font).Width;
            }
        }
    }
}