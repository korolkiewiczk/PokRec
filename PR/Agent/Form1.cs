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
using Agent.Properties;
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
                _currentProject = SaveLoad.LoadProject(Path.Combine(openFileDialog.InitialDirectory, openFileDialog.FileName));
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

                var board = new Board
                {
                    Name = result,
                    Rect = bounds
                };

                bmp.Save(SaveLoad.GetBoardPath(_currentProject, board));

                _currentProject.Boards.Add(board);

                SaveProject();

                textBoxMessage.Text = $"Captured window - {title}";
            }

            _capture = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Deactivate += Capture;
            buttonAddBoard.Enabled = false;
            buttonBoards.Enabled = false;

            buttonStop.Enabled = false;

            tabPlay.Enabled = false;

            numericSavedImagesPerBoard.Value = Settings.Default.SavedImages;
            numericInterval.Value = Settings.Default.UpdateInterval;
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
            timerGame.Interval = (int)numericInterval.Value;
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

                    board.Generated = (int)((board.Generated + 1) % numericSavedImagesPerBoard.Value);

                    bmp.Save(SaveLoad.GetBoardPathIter(_currentProject, board));

                    SaveProject();
                }
            }
        }

        private void numericInterval_ValueChanged(object sender, EventArgs e)
        {
            timerGame.Interval = (int)numericInterval.Value;
            Settings.Default.UpdateInterval = (int)numericInterval.Value;
            Settings.Default.Save();
        }

        private void numericSavedImagesPerBoard_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.SavedImages = (int)numericSavedImagesPerBoard.Value;
            Settings.Default.Save();
        }

        private void buttonTakeShot_Click(object sender, EventArgs e)
        {
            TakeShot();
        }
    }
}
