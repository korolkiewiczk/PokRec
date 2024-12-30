using System;
using System.Drawing;
using System.Windows.Forms;
using scr;
using Common;
using System.IO;
using emu.lib;

namespace MarkItDown
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _regionsMarker = new RegionsMarker();
        }

        private bool _mouseDown;
        private Point _mouseDownPoint = Point.Empty;
        private Point _mousePoint = Point.Empty;
        private Image _myImg;

        private string _baseClassesPath;
        private string _baseRegionsPath;

        private bool _scrModeOn;
        private readonly RegionsMarker _regionsMarker;
        private Rectangle _rectange;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _mouseDown = true;
            _mousePoint = _mouseDownPoint = e.Location;
        }

        private static Bitmap GetControlImage(Control ctl)
        {
            Bitmap bm = new Bitmap(ctl.Width, ctl.Height);
            ctl.DrawToBitmap(bm,
                new Rectangle(0, 0, ctl.Width, ctl.Height));
            return bm;
        }

        private static Bitmap GetFormImageWithoutBorders(Form frm, Rectangle rect)
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
                // Get refined rectangle based on edge detection
                Bitmap bm = GetFormImageWithoutBorders(this, window);
                Rectangle refinedWindow =
                    ModifierKeys.HasFlag(Keys.Shift) ? CVUtils.GetRefinedRectangle(bm, window) : window;
                if (refinedWindow != window)
                {
                    bm = GetFormImageWithoutBorders(this, refinedWindow);
                }

                bm.Save(Paths.TempImg);
                bm.Dispose();
                using (var selector = new ClassSelector(ClassesRootFolder))
                {
                    selector.ShowDialog();
                }
            }
            else
            {
                var rectangle = new Rectangle(0, 0, _myImg.Width, _myImg.Height);
                Bitmap bm = GetFormImageWithoutBorders(this, rectangle);
                var g = Graphics.FromImage(bm);
                g.DrawRectangle(new Pen(Color.Red) {Width = 4}, window);
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

        private string ClassesRootFolder => _baseClassesPath ?? $"{_myImg.Width}X{_myImg.Height}";
        private string RegionsRootFolder => _baseRegionsPath ?? $"{_myImg.Width}X{_myImg.Height}";

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

            _regionsMarker.PaintRegions(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            string imgPath = args.Length > 1 ? args[1] : Paths.CaptureImg;
            if (!File.Exists(imgPath))
            {
                MessageBox.Show($"Image file not found: {imgPath}. Please use Refresh button to capture new image",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                //_baseClassesPath = args[3];
                _rectange = (Rectangle) new RectangleConverter().ConvertFromString(args[3])!;
            }

            // MessageBox.Show(
            //     $"{args.Length}\n{imgPath}\n{_baseRegionsPath}\n{_baseClassesPath}\n{_rectange}\n{string.Join(",", args)}");

            Width = _myImg.Width;
            Height = _myImg.Height + 80;

            DoubleBuffered = true;
            Text = ClassesRootFolder + 
                   " - [c]apture window, capure current [v]iew, [s]how regions, [F2] to save regions, [arrows] to move regions";

            KeyDown += (o, eventArgs) =>
            {
                switch (eventArgs.KeyCode)
                {
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Left:
                    case Keys.Right:
                        HandleArrows(eventArgs.KeyCode);
                        break;
                    case Keys.F2:
                        //save rectangles using _regionsMarker
                        _regionsMarker.SaveRegions(RegionsRootFolder);
                        break;
                }
            };

            KeyPress += (s, ke) =>
            {
                switch (ke.KeyChar)
                {
                    case 'c':
                        SetCaptureMode(true);
                        break;
                    case 'v':
                        CaptureNewScreenshot(true);
                        break;
                    case 's':
                        _regionsMarker.MarkRegionsOnScreen(RegionsRootFolder);
                        Invalidate();
                        break;
                    
                }
            };

            LostFocus += (s, ev) => SetCaptureMode(false);
        }

        private void HandleArrows(Keys key)
        {
            if (!_regionsMarker.IsShownRegions) return;

            int dx = 0, dy = 0;
            switch (key)
            {
                case Keys.Up:
                    dy = -1;
                    break;
                case Keys.Down:
                    dy = 1;
                    break;
                case Keys.Left:
                    dx = -1;
                    break;
                case Keys.Right:
                    dx = 1;
                    break;
            }

            _regionsMarker.MoveRegions(dx, dy);

            Invalidate();
        }

        private void SetCaptureMode(bool set)
        {
            if (_scrModeOn == set) return;
            _scrModeOn = set;

            UpdateCaptureStatus();

            if (!_scrModeOn)
            {
                CaptureNewScreenshot();
            }
        }

        private void UpdateCaptureStatus()
        {
            Text = _scrModeOn ? "[Capturing]" : ClassesRootFolder;
        }

        private void CaptureNewScreenshot(bool useRect = false)
        {
            DisposeCurrentImage();

            var captureArea = useRect ? _rectange : (Rectangle?) null;
            var title = "";
            using var bmp = captureArea.HasValue
                ? ScreenShot.Capture(captureArea.Value)
                : ScreenShot.Capture(out _, out title);

            if (bmp == null)
            {
                Text = "Not captured";
                return;
            }

            ProcessCapturedImage(bmp, captureArea.HasValue ? "" : title);
        }

        private void DisposeCurrentImage()
        {
            if (_myImg == null) return;
            _myImg.Dispose();
            _myImg = null;
        }

        private void ProcessCapturedImage(Bitmap bmp, string title)
        {
            try
            {
                _myImg = new Bitmap(bmp);
                _myImg.Save(Paths.CaptureImg);
                Text = $"{ClassesRootFolder} - {title}";
                Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving capture: {ex.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Text = "Capture failed";
            }
        }
    }
}