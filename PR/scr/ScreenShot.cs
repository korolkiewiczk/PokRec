using System;
using System.Drawing;
using System.Runtime.InteropServices;
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
            IntPtr handle = GetForegroundWindow();
            StringBuilder sb = new StringBuilder(1000);
            GetWindowText(handle, sb, 1000);
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
                var foregroundWindowsHandle = GetForegroundWindow();
                var rect = new Rect();
                GetWindowRect(foregroundWindowsHandle, ref rect);
                bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
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

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
    }
}
