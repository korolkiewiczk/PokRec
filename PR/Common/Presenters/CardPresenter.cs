using System.Collections.Generic;
using System.Drawing;
using PT.Poker.Model;

namespace Common.Presenters
{
    public class CardPresenter : IResultPresenter<List<Card>>
    {
        public void Present(GameResult result, Environment e)
        {
            var rect = MapRect(result.ItemRectangle, e);

            e.Graphics.DrawRectangle(new Pen(Color.Black), rect);
            e.Graphics.DrawString(string.Join(",", result.Results),
                new Font(FontFamily.GenericMonospace, 8), Brushes.Black,
                rect.X, rect.Y);
        }

        public void Present(List<Card> result, GameResult gameResult, Environment e)
        {
            var rect = MapRect(gameResult.ItemRectangle, e);

            e.Graphics.DrawRectangle(new Pen(Color.Black), rect);
            e.Graphics.DrawString(string.Join(",", gameResult.Results),
                new Font(FontFamily.GenericMonospace, 8), Brushes.Black,
                rect.X, rect.Y);
        }

        private Rectangle MapRect(Rectangle originalRect, Environment e)
        {
            double xRatio = (double) e.ScreenRectangle.Width / e.Board.Rect.Width;
            double yRatio = (double) e.ScreenRectangle.Height / e.Board.Rect.Height;

            return new Rectangle((int) (xRatio * originalRect.X), (int) (yRatio * originalRect.Y),
                (int) (xRatio * originalRect.Width), (int) (yRatio * originalRect.Height));
        }
    }
}