using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Agent.Properties;
using Common;
using Game.Games;
using Game.Games.TexasHoldem.Model;
using Game.Games.TexasHoldem.Solving;
using Game.Games.TexasHoldem.Utils;
using scr;

namespace Agent
{
    public partial class Form1 : Form
    {
        private const string ProjectsDir = "projects";
        private Project _currentProject;
        private bool _capture;
        private bool _capturePosition;
        private readonly List<(Board,Poker,GameProcessing)> _games = new();


        public Form1()
        {
            InitializeComponent();
            var logRepository = log4net.LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            log4net.Config.XmlConfigurator.Configure(logRepository, new FileInfo("emulog.config"));
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

            using var bmp = ScreenShot.Capture(out var bounds, out var title);
            if (bmp == null) return;

            var boardName = InputDialog.ShowInputDialog("Enter board name");
            if (string.IsNullOrEmpty(boardName))
            {
                textBoxMessage.Text = "Not captured. Try again.";
                return;
            }

            var numPlayers = InputDialog.ShowInputDialog("Enter number of players (i.e. 2,6,9,10)");
            if (string.IsNullOrEmpty(numPlayers) || !int.TryParse(numPlayers, out var players))
            {
                textBoxMessage.Text = "Not captured. Try again.";
                return;
            }

            var board = new Board
            {
                Name = boardName,
                Rect = bounds,
                Settings = [new(nameof(PokerBoardSettingsParser.Players), players.ToString())]
            };

            bmp.Save(SaveLoad.GetBoardPath(_currentProject, board));

            _currentProject.Boards.Add(board);

            SaveProject();

            textBoxMessage.Text = $"Captured window - {title}";

            ScreenShot.MarkWindow(bounds);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Deactivate += CaptureWindow;
            buttonAddBoard.Enabled = false;
            buttonBoards.Enabled = false;

            buttonStop.Enabled = false;

            tabPlay.Enabled = false;

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
            StartProcessing();

            textBoxMessage.Text = "Capturing";
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }

        private void StartProcessing()
        {
            var boards = _currentProject.Boards;
            _games.Clear();
            foreach (var board in boards)
            {
                var poker = new Poker(board);
                poker.DebugFlags = PokerDebugFlags.StateResults;
                var regionSpecs = poker.GetRegionSpecs();
                foreach (var regionSpec in regionSpecs)
                {
                    regionSpec.Rectangle = RegionLoader.LoadRegion(_currentProject, board, regionSpec.Name);
                }
                var gameProcessing = new GameProcessing(board.Name, board.Rect, regionSpecs);
                poker.SetState(gameProcessing.State);
                gameProcessing.Start();
                _games.Add((board, poker, gameProcessing));
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            foreach (var (_, _, gameProcessing) in _games)
            {
                gameProcessing.DisposeAsync().ConfigureAwait(true);
            }

            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }

        private void buttonShowGame_Click(object sender, EventArgs e)
        {
            foreach (var (_, poker, gameProcessing) in _games)
            {
                var game = new Game(poker, gameProcessing);
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