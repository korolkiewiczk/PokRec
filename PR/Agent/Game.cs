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
            timer.Interval = 50;
            timer.Tick += TimerTick;
            timer.Enabled = true;

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer, true);

            ResizeEnd += Form1_CreateBackBuffer;
            Load += Form1_CreateBackBuffer;
            Paint += Form1_Paint;
        }

        public void BoardUpdated()
        {
            _pokerGame.Process();

            Text = DateTime.Now.ToString() + " " + _board.Generated;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Draw();
        }

        private void Draw()
        {
            if (_backbuffer != null)
            {
                using (var g = Graphics.FromImage(_backbuffer))
                {
                    if (_pokerGame.GameResults.ContainsKey(_board.Generated))
                    {
                        g.Clear(BackColor);
                        var results = _pokerGame.GameResults[_board.Generated];

                        foreach (var gameResult in results)
                        {
                            gameResult.Presenter.Present(gameResult,
                                new Environment(g, new Rectangle(0, 0, Width, Height), _board));
                        }
                        
                        _pokerGame.ShowMatch(_board.Generated, new Environment(g, new Rectangle(0, 0, Width, Height), _board));
                    }
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