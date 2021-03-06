﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Common;

namespace Agent
{
    public partial class ManageBoards : Form
    {
        private readonly string _projectDirectory;
        private string _name;
        private Project _project;

        public ManageBoards(string projectDirectory, string name)
        {
            _projectDirectory = projectDirectory;
            _name = name;
            InitializeComponent();
        }

        private void ManageBoards_Load(object sender, EventArgs e)
        {
            _project = SaveLoad.LoadProject(Path.Combine(_projectDirectory, _name));
            if (!_project.Boards.Any())
            {
                MessageBox.Show("First add at least one board");
                Close();
                return;
            }
            RefreshListbox();
        }

        private void RefreshListbox()
        {
            foreach (var board in _project.Boards)
            {
                listBox1.Items.Add(board);
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            IntPtr desktopPtr = GetDC(IntPtr.Zero);
            Graphics g = Graphics.FromHdc(desktopPtr);

            const int width = 4;
            var r = SelectedBoard.Rect;
            g.DrawRectangle(new Pen(Color.Red, width), new Rectangle(r.X - width, r.Y - width, r.Width + 2 * width, r.Height + 2 * width));

            g.Dispose();
            ReleaseDC(IntPtr.Zero, desktopPtr);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            _project.Boards.Remove(SelectedBoard);
            listBox1.Items.Clear();
            RefreshListbox();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            MarkSelectedBoard();
        }

        private void MarkSelectedBoard()
        {
            Process.Start("MarkItDown.exe", $@"""{SaveLoad.GetBoardPath(_project, SelectedBoard)}""");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            using (var bmp = scr.ScreenShot.Capture(SelectedBoard.Rect))
            {
                bmp.Save(SaveLoad.GetBoardPath(_project,SelectedBoard));
            }

            MarkSelectedBoard();
        }

        private Board SelectedBoard => (Board) (listBox1.SelectedItem as Board ?? listBox1.Items[0]);

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveLoad.SaveProject(_project);
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                Process.Start("MarkItDown.exe", $@"""{file}""");
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            Process.Start(Path.GetDirectoryName(SaveLoad.GetBoardPath(_project, SelectedBoard)));
        }

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);
    }
}
