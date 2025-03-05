using System.Drawing;
using Common;
using Game.Common;

namespace Game.Presentation
{
    public static class Util
    {
        public static Rectangle MapRect(Rectangle originalRect, GameEnvironment e)
        {
            double xRatio = (double) e.ScreenRectangle.Width / e.Board.Rect.Width;
            double yRatio = (double) e.ScreenRectangle.Height / e.Board.Rect.Height;

            return new Rectangle((int) (xRatio * originalRect.X), (int) (yRatio * originalRect.Y),
                (int) (xRatio * originalRect.Width), (int) (yRatio * originalRect.Height));
        }
    }
}