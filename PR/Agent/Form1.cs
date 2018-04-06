using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using scr;

namespace Agent
{
    public partial class Form1 : Form
    {
        private const string ProjectsDir = "projects";
        private const string BoardsDir = "boards";
        private Project _currentProject;
        private bool _capture;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonNewPrj_Click(object sender, EventArgs e)
        {
            var result = Interaction.InputBox("Enter project name");
            if (string.IsNullOrEmpty(result)) return;

            _currentProject = new Project()
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
                _currentProject = ProjectStore(openFileDialog.InitialDirectory).LoadProject(openFileDialog.FileName);
                labelCurrentPrj.Text = _currentProject.Name;
                buttonAddBoard.Enabled = true;
                buttonBoards.Enabled = true;
                tabPlay.Enabled = true;
                textBoxMessage.Text = $"Loaded project {_currentProject.Name}";
            }
        }

        private void SaveProject()
        {
            ProjectStore(_currentProject.Path).SaveProject(_currentProject);
        }

        private static string ProjectDirectory => Path.Combine(Directory.GetCurrentDirectory(), ProjectsDir);

        private void buttonAddBoard_Click(object sender, EventArgs e)
        {
            textBoxMessage.Text = "Select window";
            _capture = true;
        }

        private void Capture(object sender, EventArgs args)
        {
            if (!_capture) return;

            Rectangle bounds;
            string title;
            using (var bmp = ScreenShot.Capture(out bounds, out title))
            {
                if (bmp == null) return;

                var result = Interaction.InputBox("Enter board name");
                if (string.IsNullOrEmpty(result))
                {
                    _capture = false;
                    textBoxMessage.Text = "Not captured. Try again.";
                    return;
                }

                var boardNameFromBounds = BoardNameFromBounds(bounds, result);
                var boardsDir = Path.Combine(_currentProject.Path, BoardsDir);
                var boardPath = Path.Combine(boardsDir, boardNameFromBounds);
                if (!Directory.Exists(boardsDir))
                {
                    Directory.CreateDirectory(boardsDir);
                }

                bmp.Save(boardPath);

                var board = new Board
                {
                    Name = boardPath,
                    Rect = bounds
                };

                _currentProject.Boards.Add(board);

                SaveProject();

                textBoxMessage.Text = $"Captured window - {title}";
            }

            _capture = false;
        }

        private string BoardNameFromBounds(Rectangle bounds, string name)
        {
            return $"board{bounds.Width}X{bounds.Height} {name}.png";
        }

        private static SaveLoad ProjectStore(string path) => new SaveLoad(path);

        private void Form1_Load(object sender, EventArgs e)
        {
            Deactivate += Capture;
            buttonAddBoard.Enabled = false;
            buttonBoards.Enabled = false;
            tabPlay.Enabled = false;
        }

        private void buttonBoards_Click(object sender, EventArgs e)
        {
            new ManageBoards(ProjectDirectory, _currentProject.Name).ShowDialog();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            timerGame.Interval = (int) numericInterval.Value;
            timerGame.Start();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            timerGame.Stop();
        }

        private void timerGame_Tick(object sender, EventArgs e)
        {
            textBoxMessage.Text += ".";
        }

        private void numericInterval_ValueChanged(object sender, EventArgs e)
        {
            timerGame.Interval = (int) numericInterval.Value;
        }
    }
}
