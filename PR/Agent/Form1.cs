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

namespace Agent
{
    public partial class Form1 : Form
    {
        private Project _currentProject;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonNewPrj_Click(object sender, EventArgs e)
        {
            var result = Interaction.InputBox("Enter project name");
            _currentProject = new Project()
            {
                Name = result,
                Boards = new Boards()
            };
            labelCurrentPrj.Text = result;
            new SaveLoad("projects").SaveProject(_currentProject);
            //File.WriteAllText(_currentProject.Name+".proj"); JsonConvert.SerializeObject(_currentProject);
        }

        private void buttonLoadPrj_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog=new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = $@"*{SaveLoad.ProjExtension}|*{SaveLoad.ProjExtension}";
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")
            {
                _currentProject = new SaveLoad("projects").LoadProject(openFileDialog.FileName);
                labelCurrentPrj.Text = _currentProject.Name;
            }
        }

        private void buttonAddBoard_Click(object sender, EventArgs e)
        {
            labelMessage.Text = "Select window";
        }
    }
}
