using System;
using System.Drawing;
using System.Windows.Forms;
using Common;
using Game.Games;
using Game.Games.TexasHoldem.Presenters;
using Game.Games.TexasHoldem.Solving;
using Game.Presenters;
using Environment = Game.Common.Environment;

namespace Agent
{
    public partial class Game : Form
    {
        private Bitmap _backbuffer;

        private readonly Board _board;
        private readonly PokerPresenter _pokerPresenter;

        private Game()
        {
            InitializeComponent();
        }

        public Game(Poker poker) : this()
        {
            _board = poker.Board;
            _pokerPresenter = new PokerPresenter(poker);

            Timer timer = new Timer();
            timer.Interval = 100;
            timer.Tick += TimerTick_GameProcessing;
            timer.Enabled = true;

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer, true);

            ResizeEnd += Form1_CreateBackBuffer;
            Load += Form1_CreateBackBuffer;
            Paint += Form1_Paint;
        }

        private void TimerTick_GameProcessing(object sender, EventArgs e)
        {
            if (_backbuffer == null) return;
            
            using (var g = Graphics.FromImage(_backbuffer))
            {
                g.Clear(BackColor);

                _pokerPresenter.Show(new Environment(g, new Rectangle(0, 0, Width, Height), _board));
            }

            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (_backbuffer != null)
            {
                e.Graphics.DrawImageUnscaled(_backbuffer, Point.Empty);
            }
        }

        private void Form1_CreateBackBuffer(object sender, EventArgs e)
        {
            _backbuffer?.Dispose();

            _backbuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
        }
    }
}