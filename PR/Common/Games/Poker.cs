using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.RegionMatchers;

namespace Common.Games
{
    public class Poker : GameBase
    {
        private Flop _flop;

        public Poker(Board board) : base(board)
        {
            InitializeMatchers();
        }

        private void InitializeMatchers()
        {
            _flop = new Flop(_board);
        }

        protected override void Analize()
        {
            var flopMatch = _flop.Match();
            if (flopMatch == "As")
            {

            }
        }
    }
}
