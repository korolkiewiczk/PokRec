using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;

namespace scr
{
    public class ScreenShot
    {
        public enum ScreenCaptureMode
        {
            Screen,
            Window
        }

        public static string GetTitleOfForegroundWindow()
        {
            IntPtr handle = Interop.GetForegroundWindow();
            StringBuilder sb = new StringBuilder(1000);
            Interop.GetWindowText(handle, sb, 1000);
            return sb.ToString();
        }

        public static Bitmap Capture(out Rectangle bounds, out string title, ScreenCaptureMode screenCaptureMode = ScreenCaptureMode.Window)
        {
            if (screenCaptureMode == ScreenCaptureMode.Screen)
            {
                bounds = Screen.GetBounds(Point.Empty);
            }
            else
            {
                bounds = CaptureWindowRect();
            }

            title = GetTitleOfForegroundWindow();

            return Capture(bounds);
        }

        public static Bitmap Capture(Rectangle bounds)
        {
            if (bounds.Width == 0 || bounds.Height == 0)
            {
                return null;
            }

            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var g = Graphics.FromImage(result))
            {
                g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }

            return result;
        }

        public static Rectangle CaptureWindowRect()
        {
            Point point;
            Interop.GetCursorPos(out point);
            var foregroundWindowsHandle = Interop.WindowFromPoint(point);
            var rect = new Rect();
            Interop.GetWindowRect(foregroundWindowsHandle, ref rect);
            return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        public static void MarkWindow(Rectangle rect)
        {
            IntPtr desktopPtr = Interop.GetDC(IntPtr.Zero);
            Graphics g = Graphics.FromHdc(desktopPtr);

            const int width = 4;

            g.DrawRectangle(new Pen(Color.Red, width),
                new Rectangle(rect.X - width, rect.Y - width, rect.Width + 2 * width, rect.Height + 2 * width));

            g.Dispose();
            Interop.ReleaseDC(IntPtr.Zero, desktopPtr);
        }
    }
}
