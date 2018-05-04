using System.Collections.Generic;
using System.Drawing;

namespace Common
{
    public class GameResult
    {
        public GameResult(string name, Rectangle itemRectangle, List<string> results, IResultPresenter presenter)
        {
            ItemRectangle = itemRectangle;
            Name = name;
            Results = results;
            Presenter = presenter;
        }

        public string Name { get; set; }
        public Rectangle ItemRectangle { get; }
        public List<string> Results { get; }
        public IResultPresenter Presenter { get; }
    }
}