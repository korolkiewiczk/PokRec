using System;
using System.Drawing;
using System.Windows.Forms;
using scr;
using Common;
using System.IO;

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

        private string _baseClassesPath;
        private string _baseRegionsPath;
        
        private bool _scrModeOn;

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

                bm.Save(Paths.TempImg);
                bm.Dispose();
                using (var selector = new ClassSelector(ClassesRootFolder))
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
                bm.Save(Paths.TempImg);
                bm.Dispose();
                using (var selector = new RegionSelector(RegionsRootFolder, window))
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

        private string ClassesRootFolder => _baseClassesPath?? $"{_myImg.Width}X{_myImg.Height}";
        private string RegionsRootFolder => _baseRegionsPath?? $"{_myImg.Width}X{_myImg.Height}";

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

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            string imgPath = args.Length > 1 ? args[1] : Paths.CaptureImg;
            if (!File.Exists(imgPath))
            {
                MessageBox.Show($"Image file not found: {imgPath}. Please use Refresh button to capture new image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            _myImg = Image.FromFile(imgPath);
            
            if (args.Length == 3)
            {
                _baseRegionsPath = args[2];
            }
            
            if (args.Length == 4)
            {
                _baseRegionsPath = args[2];
                _baseClassesPath = args[3];
            }

            Width = _myImg.Width;
            Height = _myImg.Height + 80;

            DoubleBuffered = true;
            Text = ClassesRootFolder;

            KeyPress += (s, ke) =>
            {
                if (ke.KeyChar == 's')
                {
                    SetCaptureMode(true);
                }
            };

            LostFocus += (s, ev) => SetCaptureMode(false);
        }

        private void SetCaptureMode(bool set)
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
                    bmp.Save(Paths.CaptureImg);
                    _myImg.Dispose();
                }

                _myImg = Image.FromFile(Paths.CaptureImg);
                Text = $"{ClassesRootFolder} - {title}";
            }
        }
    }
}
