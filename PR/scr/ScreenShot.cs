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
            //EnsureDpiAwareness();

            Point point;
            Interop.GetCursorPos(out point);
            var foregroundWindowsHandle = Interop.WindowFromPoint(point);
            var rect = new Rect();
            Interop.GetWindowRect(foregroundWindowsHandle, ref rect);
            // Adjust for DPI scaling
            float scaleFactor = GetDpiScaleFactor(foregroundWindowsHandle);
            if (scaleFactor != 1.0f)
            {
                // Adjust for DPI scaling
                int adjustedLeft = (int)(rect.Left * scaleFactor);
                int adjustedTop = (int)(rect.Top * scaleFactor);
                int adjustedRight = (int)(rect.Right * scaleFactor);
                int adjustedBottom = (int)(rect.Bottom * scaleFactor);

                // Return Rectangle with corrected scaling
                return new Rectangle(
                    adjustedLeft,
                    adjustedTop,
                    adjustedRight - adjustedLeft,
                    adjustedBottom - adjustedTop
                );
            }
            else
            {
                // No scaling needed, return as-is
                return new Rectangle(
                    rect.Left,
                    rect.Top,
                    rect.Right - rect.Left,
                    rect.Bottom - rect.Top
                );
            }
        }


        private static void EnsureDpiAwareness()
        {
            // Set the process DPI awareness to per-monitor DPI aware
            if (!Interop.SetProcessDpiAwarenessContext(Interop.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE))
            {
                Console.WriteLine("Failed to set DPI awareness context.");
            }
        }

        private static float GetDpiScaleFactor(IntPtr hwnd)
        {
            // Get DPI for the specific window
            uint dpi = Interop.GetDpiForWindow(hwnd);
            const float standardDpi = 96f; // 96 DPI is standard scale
            return dpi / standardDpi;
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

        public static void MoveAndResizeWindow(Point newPosition, Size newSize)
        {
            Point point;
            Interop.GetCursorPos(out point);
            var foregroundWindowsHandle = Interop.WindowFromPoint(point);
            // SWP_NOZORDER flag keeps the window's position in the Z order
            // SWP_SHOWWINDOW ensures the window is shown after moving
            const int SWP_NOZORDER = 0x0004;
            const int SWP_SHOWWINDOW = 0x0040;

            var dpiScale = GetDpiScaleFactor(foregroundWindowsHandle);
            Interop.SetWindowPos(
                foregroundWindowsHandle,
                IntPtr.Zero,
                (int)(newPosition.X / dpiScale),
                (int)(newPosition.Y / dpiScale),
                (int)(newSize.Width / dpiScale),
                (int)(newSize.Height / dpiScale),
                SWP_NOZORDER | SWP_SHOWWINDOW
            );
        }
    }
}
