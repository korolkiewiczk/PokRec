using System.Collections.Generic;
using System.Drawing;

namespace Common
{
    public class GameResult
    {
        public GameResult(string name, Rectangle itemRectangle, List<string> results)
        {
            ItemRectangle = itemRectangle;
            Name = name;
            Results = results;
        }

        public string Name { get; set; }
        public Rectangle ItemRectangle { get; }
        public List<string> Results { get; }
    }
}