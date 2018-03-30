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
        private Project _currentProject;
        private bool _scrModeOn;

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
                Name = result,
                Boards = new Boards()
            };
            labelCurrentPrj.Text = result;
            new SaveLoad(ProjectsDir).SaveProject(_currentProject);
            //File.WriteAllText(_currentProject.Name+".proj"); JsonConvert.SerializeObject(_currentProject);
        }

        private void buttonLoadPrj_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = $@"*{SaveLoad.ProjExtension}|*{SaveLoad.ProjExtension}";
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")
            {
                _currentProject = new SaveLoad(ProjectsDir).LoadProject(openFileDialog.FileName);
                labelCurrentPrj.Text = _currentProject.Name;
            }
        }

        private void buttonAddBoard_Click(object sender, EventArgs e)
        {
            labelMessage.Text = "Select window";
            this.LostFocus += Capture;
        }

        private void Capture(object sender, EventArgs args)
        {
            Rectangle bounds;
            using (var bmp = ScreenShot.Capture(out bounds))
            {
                var boardNameFromBounds = BoardNameFromBounds(bounds);
                bmp.Save(boardNameFromBounds);

                var board = new Board();
                _currentProject.Boards.Add(board);

                labelMessage.Text = "Captured " + boardNameFromBounds;
            }

            this.LostFocus -= Capture;
        }

        private string BoardNameFromBounds(Rectangle bounds)
        {
            return "board";
            //projects/prj1.proj - Name, Boards
        }
    }
}
