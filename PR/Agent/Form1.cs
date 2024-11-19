using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Agent.Properties;
using Common;
using Game.Games;
using scr;

namespace Agent
{
    public partial class Form1 : Form
    {
        private const string ProjectsDir = "projects";
        private Project _currentProject;
        private bool _capture;
        private bool _capturePosition;

        private Dictionary<Board, IBoardObserver> _boardObservers;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonNewPrj_Click(object sender, EventArgs e)
        {
            var result = InputDialog.ShowInputDialog("Enter project name");
            if (string.IsNullOrEmpty(result)) return;

            _currentProject = new Project
            {
                Path = ProjectDirectory,
                Name = result,
                Boards = new Boards()
            };
            labelCurrentPrj.Text = result;
            SaveProject();
            buttonAddBoard.Enabled = true;
            buttonBoards.Enabled = true;
            tabPlay.Enabled = true;

            textBoxMessage.Text = $"Created project {_currentProject.Name}";
        }

        private void buttonLoadPrj_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = ProjectDirectory;
            openFileDialog.Filter = $@"*{SaveLoad.ProjExtension}|*{SaveLoad.ProjExtension}";
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")
            {
                _currentProject =
                    SaveLoad.LoadProject(Path.Combine(openFileDialog.InitialDirectory, openFileDialog.FileName));
                labelCurrentPrj.Text = _currentProject.Name;
                buttonAddBoard.Enabled = true;
                buttonBoards.Enabled = true;
                tabPlay.Enabled = true;
                textBoxMessage.Text = $"Loaded project {_currentProject.Name}";
            }
        }

        private void SaveProject()
        {
            SaveLoad.SaveProject(_currentProject);
        }

        private static string ProjectDirectory => Path.Combine(Directory.GetCurrentDirectory(), ProjectsDir);

        private void buttonAddBoard_Click(object sender, EventArgs e)
        {
            textBoxMessage.Text = "Select window";
            _capture = true;
        }

        private void CaptureWindow(object sender, EventArgs args)
        {
            if (!_capture) return;
            _capture = false;

            Rectangle bounds;
            string title;
            using (var bmp = ScreenShot.Capture(out bounds, out title))
            {
                if (bmp == null) return;

                var boardName = InputDialog.ShowInputDialog("Enter board name");
                if (string.IsNullOrEmpty(boardName))
                {
                    textBoxMessage.Text = "Not captured. Try again.";
                    return;
                }
                
                var numPlayers = InputDialog.ShowInputDialog("Enter number of players (i.e. 2,6,9,10)");
                int players;
                if (string.IsNullOrEmpty(numPlayers) || !int.TryParse(numPlayers, out players))
                {
                    textBoxMessage.Text = "Not captured. Try again.";
                    return;
                }

                var board = new Board
                {
                    Name = boardName,
                    Rect = bounds,
                    Settings = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(nameof(PokerBoardSettingsParser.Players), players.ToString())
                    }
                };

                bmp.Save(SaveLoad.GetBoardPath(_currentProject, board));

                _currentProject.Boards.Add(board);

                SaveProject();

                textBoxMessage.Text = $"Captured window - {title}";

                ScreenShot.MarkWindow(bounds);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Deactivate += CaptureWindow;
            buttonAddBoard.Enabled = false;
            buttonBoards.Enabled = false;

            buttonStop.Enabled = false;

            tabPlay.Enabled = false;

            numericSavedImagesPerBoard.Value = Settings.Default.SavedImages;
            numericInterval.Value = Settings.Default.UpdateInterval;

            MarkItDownFiles.GenerateMarkItDownFiles();
        }

        private void buttonBoards_Click(object sender, EventArgs e)
        {
            new ManageBoards(ProjectDirectory, _currentProject.Name).ShowDialog();
            _currentProject = SaveLoad.LoadProject(Path.Combine(ProjectDirectory, _currentProject.Name));
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            timerGame.Interval = (int) numericInterval.Value;
            timerGame.Start();
            textBoxMessage.Text = "Capturing";
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            timerGame.Stop();
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }

        private void timerGame_Tick(object sender, EventArgs e)
        {
            TakeShot();

            textBoxMessage.Text += ".";
        }

        private void TakeShot()
        {
            foreach (var board in _currentProject.Boards)
            {
                using (var bmp = ScreenShot.Capture(board.Rect))
                {
                    if (bmp == null) continue;

                    board.IncrementGenerated((int) numericSavedImagesPerBoard.Value);

                    bmp.Save(SaveLoad.GetBoardPathIter(_currentProject, board));

                    SaveProject();
                }

                _boardObservers?[board]?.BoardUpdated();
            }
        }

        private void numericInterval_ValueChanged(object sender, EventArgs e)
        {
            timerGame.Interval = (int) numericInterval.Value;
            Settings.Default.UpdateInterval = (int) numericInterval.Value;
            Settings.Default.Save();
        }

        private void numericSavedImagesPerBoard_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.SavedImages = (int) numericSavedImagesPerBoard.Value;
            Settings.Default.Save();
        }

        private void buttonTakeShot_Click(object sender, EventArgs e)
        {
            TakeShot();
        }

        private void buttonShowGame_Click(object sender, EventArgs e)
        {
            _boardObservers = new Dictionary<Board, IBoardObserver>();
            foreach (var board in _currentProject.Boards)
            {
                var game = new Game(_currentProject, board);
                _boardObservers[board] = game;
                game.Show();
            }
        }

        private void buttonFixWindow_Click(object sender, EventArgs e)
        {
            _capturePosition = true;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            
            if (!_capturePosition) return;
            _capturePosition = false;

            var currentBoard = _currentProject.Boards.FirstOrDefault();
            if (currentBoard != null)
            {
                ScreenShot.MoveAndResizeWindow(currentBoard.Rect.Location, currentBoard.Rect.Size);
            }
        }
    }
}