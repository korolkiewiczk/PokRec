using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public abstract class GameBase
    {
        protected readonly Board _board;

        public GameBase(Board board)
        {
            _board = board;
        }

        protected abstract void Analize();
    }
}
