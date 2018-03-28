using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace scr
{
    public class ScreenShot
    {
        public static string GetTitleOfForegroundWindow()
        {
            IntPtr handle = GetForegroundWindow();
            StringBuilder sb = new StringBuilder(1000);
            GetWindowText(handle, sb, 1000);
            return sb.ToString();
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    }
}
