using System.Drawing;

namespace Common
{
    public class Board
    {
        public string Name { get; set; }
        public Rectangle Rect { get; set; }

        public override string ToString()
        {
            return Id;
        }

        public string Id => $"{Rect.Width}X{Rect.Height}_{Name}";
    }
}
