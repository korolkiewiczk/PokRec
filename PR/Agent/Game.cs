using System;
using System.Drawing;
using System.Windows.Forms;
using Common;
using Game.Common;
using Game.Presentation;
using Game.Solving;

namespace Agent
{
    public partial class Game : Form
    {
        private Bitmap _backbuffer;

        private readonly Poker _poker;
        private readonly GameProcessing _gameProcessing;

        private Game()
        {
            InitializeComponent();
        }

        public Game(Poker poker, GameProcessing gameProcessing) : this()
        {
            _poker = poker;
            _gameProcessing = gameProcessing;

            _gameProcessing.ProcessingCompleted += GameProcessing_ProcessingCompleted;

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer, true);

            ResizeEnd += Form1_CreateBackBuffer;
            Load += Form1_CreateBackBuffer;
            Paint += Form1_Paint;
        }

        private void GameProcessing_ProcessingCompleted(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(Render);
                return;
            }
            Render();
        }

        private void Render()
        {
            if (_backbuffer == null) return;
            
            using (var g = Graphics.FromImage(_backbuffer))
            {
                g.Clear(BackColor);
                var pokerResults = _poker.Solve();
                PokerPresenter.Show(new GameEnvironment(g, new Rectangle(0, 0, Width, Height), _poker.Board),
                    pokerResults, _poker.GameActions, _poker.StartingBets, _poker.PlayerStats);
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