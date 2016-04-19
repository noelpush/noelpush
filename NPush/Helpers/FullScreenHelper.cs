using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NoelPush.Helpers
{
    internal static class FullScreenHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static bool IsFullScreen
        {
            get
            {
                var desktopHandle = GetDesktopWindow();
                var shellHandle = GetShellWindow();
                var hWnd = GetForegroundWindow();

                if (!hWnd.Equals(IntPtr.Zero))
                {
                    if (!(hWnd.Equals(desktopHandle) || hWnd.Equals(shellHandle)))
                    {
                        RECT appBounds;
                        GetWindowRect(hWnd, out appBounds);
                        var screenBounds = Screen.FromHandle(hWnd).Bounds;

                        return (appBounds.Bottom - appBounds.Top) == screenBounds.Height && (appBounds.Right - appBounds.Left) == screenBounds.Width;
                    }
                }

                return false;
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowRect(IntPtr hwnd, out RECT rc);
    }
}
