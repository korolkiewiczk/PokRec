using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
            _project = new SaveLoad(_projectDirectory).LoadProject(_name);
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

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

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
            Process.Start("MarkItDown.exe", $@"""{SelectedBoard.Name}""");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            using (var bmp = scr.ScreenShot.Capture(SelectedBoard.Rect))
            {
                bmp.Save(SelectedBoard.Name);
            }

            MarkSelectedBoard();
        }

        private Board SelectedBoard => listBox1.SelectedItem as Board;
    }
}
