using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.RegionMatchers
{
    public class Flop : IRegionMatcher<string> //todo card
    {
        private Rectangle _rectangle;

        public Flop(Board board)
        {
            _rectangle = RegionLoader.LoadRegion("", board, "flop");
        }

        public string Match()
        {
            return "Ah";
        }
    }
}
