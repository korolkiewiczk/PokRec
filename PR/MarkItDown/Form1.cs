using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using scr;

namespace MarkItDown
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private bool _mouseDown;
        private Point _mouseDownPoint = Point.Empty;
        private Point _mousePoint = Point.Empty;
        private Image _myImg;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _mouseDown = true;
            _mousePoint = _mouseDownPoint = e.Location;
        }

        private Bitmap GetControlImage(Control ctl)
        {
            Bitmap bm = new Bitmap(ctl.Width, ctl.Height);
            ctl.DrawToBitmap(bm,
                new Rectangle(0, 0, ctl.Width, ctl.Height));
            return bm;
        }

        private Bitmap GetFormImageWithoutBorders(Form frm, Rectangle rect)
        {
            // Get the form's whole image.
            using (Bitmap whole_form = GetControlImage(frm))
            {
                // See how far the form's upper left corner is
                // from the upper left corner of its client area.
                Point origin = frm.PointToScreen(new Point(0, 0));
                int dx = origin.X - frm.Left;
                int dy = origin.Y - frm.Top;

                // Copy the client area into a new Bitmap.
                Rectangle newRect = new Rectangle(dx + rect.X, dy + rect.Y, rect.Width, rect.Height);
                int wid = newRect.Width;
                int hgt = newRect.Height;
                Bitmap bm = new Bitmap(wid, hgt);
                using (Graphics gr = Graphics.FromImage(bm))
                {
                    gr.DrawImage(whole_form, 0, 0,
                        newRect,
                        GraphicsUnit.Pixel);
                }

                return bm;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _mouseDown = false;

            Rectangle window = GetMarkedWindow();

            if (window.Size.Width == 0 || window.Size.Height == 0) return;

            if (e.Button == MouseButtons.Left)
            {
                Bitmap bm = GetFormImageWithoutBorders(this, window);

                bm.Save(Common.TempImg);
                bm.Dispose();
                using (var selector = new ClassSelector(RootFolder))
                {
                    selector.ShowDialog();
                }
            }
            else
            {
                Bitmap bm = GetFormImageWithoutBorders(this, new Rectangle(0, 0, _myImg.Width, _myImg.Height));
                var g = Graphics.FromImage(bm);
                g.DrawRectangle(new Pen(Color.Red) { Width = 4 }, window);
                g.Dispose();
                bm.Save(Common.TempImg);
                bm.Dispose();
                using (var selector = new RegionSelector(RootFolder, window))
                {
                    selector.ShowDialog();
                }
            }
        }

        private Rectangle GetMarkedWindow()
        {
            return new Rectangle(
                Math.Min(_mouseDownPoint.X, _mousePoint.X),
                Math.Min(_mouseDownPoint.Y, _mousePoint.Y),
                Math.Abs(_mouseDownPoint.X - _mousePoint.X),
                Math.Abs(_mouseDownPoint.Y - _mousePoint.Y));
        }

        private string RootFolder => _myImg.Width + "X" + _myImg.Height;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _mousePoint = e.Location;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawImage(_myImg, new Rectangle(0, 0, _myImg.Width, _myImg.Height));

            if (_mouseDown)
            {
                Rectangle window = GetMarkedWindow();

                SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 255));
                e.Graphics.FillRegion(semiTransBrush, new Region(window));
            }
        }

        private bool _scrModeOn = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            _myImg = Image.FromFile(args.Length > 1 ? args[1] : @"recon\board\board4.png");

            Width = _myImg.Width;
            Height = _myImg.Height + 80;

            DoubleBuffered = true;
            Text = RootFolder;

            KeyPress += (s, ke) =>
            {
                if (ke.KeyChar == 's')
                {
                    Capture(true);
                }
            };

            LostFocus += (s, ev) => Capture(false);
        }

        private void Capture(bool set)
        {
            if (_scrModeOn == set) return;

            _scrModeOn = set;
            if (_scrModeOn)
            {
                Text = "[Capturing]";
            }
            else
            {
                Rectangle bounds;
                string title;
                using (var bmp = ScreenShot.Capture(out bounds, out title))
                {
                    if (bmp == null)
                    {
                        Text = "Not captured";
                        return;
                    }
                    bmp.Save(Common.CaptureImg);
                    _myImg.Dispose();
                }

                _myImg = Image.FromFile(Common.CaptureImg);
                Text = $"{RootFolder} - {title}";
            }
        }
    }
}