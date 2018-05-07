using System.Drawing;
using Common;

namespace Game
{
    public class Environment
    {
        public Environment(Graphics graphics, Rectangle screenRectangle, Board board)
        {
            Graphics = graphics;
            ScreenRectangle = screenRectangle;
            Board = board;
        }

        public Graphics Graphics { get; }
        public Rectangle ScreenRectangle { get; } 
        public Board Board { get; }
    }
}