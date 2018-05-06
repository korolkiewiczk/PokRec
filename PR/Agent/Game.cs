using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Common;
using Common.Games;
using Environment = Common.Environment;

namespace Agent
{
    public partial class Game : Form, IBoardObserver
    {
        private Bitmap _backbuffer;

        private readonly Board _board;
        private readonly Poker _pokerGame;
        private bool _closed;

        public Game()
        {
            InitializeComponent();
        }

        public Game(Project prj, Board board) : this()
        {
            _board = board;
            _pokerGame = new Poker(prj, board, x => Process.Start("emu.exe", x));
            Text = board.ToString();
            Timer timer = new Timer();
            timer.Interval = 100;
            timer.Tick += TimerTick;
            timer.Enabled = true;

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer, true);

            ResizeEnd += Form1_CreateBackBuffer;
            Load += Form1_CreateBackBuffer;
            Paint += Form1_Paint;

            Closed += (s, e) => _closed = true;
        }

        public void BoardUpdated()
        {
            if (_closed)
            {
                return;
            }

            _pokerGame.Process();

            Text = $@"{DateTime.Now} {_board.Generated} {_board.Computed}";
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Draw();
        }

        private void Draw()
        {
            if (_backbuffer != null && _pokerGame.HasComputed())
            {
                using (var g = Graphics.FromImage(_backbuffer))
                {
                    g.Clear(BackColor);

                    _pokerGame.Show(new Environment(g, new Rectangle(0, 0, Width, Height), _board));
                }

                Invalidate();
            }
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
            if (_backbuffer != null)
            {
                _backbuffer.Dispose();
            }

            _backbuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
        }
    }
}