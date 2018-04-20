using System;
using System.Windows.Forms;
using Common;

namespace Agent
{
    public partial class Game : Form, IBoardObserver
    {
        private Board _board;

        public Game()
        {
            InitializeComponent();
        }

        public Game(Board board) : this()
        {
            _board = board;
            Text = board.ToString();
        }

        public void BoardUpdated()
        {
            Text = DateTime.Now.ToString() + " "+_board.Generated;
        }
    }
}
