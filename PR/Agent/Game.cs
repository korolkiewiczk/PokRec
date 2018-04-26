using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Common;
using Common.Games;

namespace Agent
{
    public partial class Game : Form, IBoardObserver
    {
        Bitmap Backbuffer;

        private readonly Board _board;
        private Poker _pokerGame;

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
            if (Backbuffer != null)
            {
                using (var g = Graphics.FromImage(Backbuffer))
                {
                    if (_pokerGame.GameResults.ContainsKey(_board.Generated))
                    {
                        var results = _pokerGame.GameResults[_board.Generated];

                        foreach (var gameResult in results)
                        {
                            var rect = MapRect(gameResult.ResultRect);
                            g.DrawRectangle(new Pen(Color.Black), rect );
                            g.DrawString(gameResult.ResultText, new Font(FontFamily.GenericMonospace, 10), Brushes.Black,
                                rect .X, rect .Y);
                        }
                    }
                }

                Invalidate();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (Backbuffer != null)
            {
                e.Graphics.DrawImageUnscaled(Backbuffer, Point.Empty);
            }
        }

        private void Form1_CreateBackBuffer(object sender, EventArgs e)
        {
            if (Backbuffer != null)
            {
                Backbuffer.Dispose();
            }

            Backbuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
        }

        private Rectangle MapRect(Rectangle originalRect)
        {
            double xRatio = (double)Width / _board.Rect.Width;
            double yRatio = (double)Height/ _board.Rect.Height;

            return new Rectangle((int) (xRatio * originalRect.X), (int) (yRatio * originalRect.Y),
                (int) (xRatio * originalRect.Width), (int) (yRatio * originalRect.Height));
        }
    }
}