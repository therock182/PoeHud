using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PoeHUD.Framework
{
    public static class WinApi
    {
        #region Methods

        public static void EnableTransparent(IntPtr handle, Rectangle size)
        {
            int windowLong = GetWindowLong(handle, GWL_EXSTYLE) | WS_EX_LAYERED | WS_EX_TRANSPARENT;
            SetWindowLong(handle, GWL_EXSTYLE, new IntPtr(windowLong));
            SetLayeredWindowAttributes(handle, 0, 255, LWA_ALPHA);
            Margins margins = Margins.FromRectangle(size);
            DwmExtendFrameIntoClientArea(handle, ref margins);
        }

        public static Rectangle GetClientRectangle(IntPtr handle)
        {
            Rect rect;
            Point point;
            GetClientRect(handle, out rect);
            ClientToScreen(handle, out point);
            return rect.ToRectangle(point);
        }

        public static bool IsForegroundWindow(IntPtr handle)
        {
            return GetForegroundWindow() == handle;
        }

        #endregion

        #region Constants

        private const int GWL_EXSTYLE = -20;

        private const int WS_EX_LAYERED = 0x80000;

        private const int WS_EX_TRANSPARENT = 0x20;

        private const int LWA_ALPHA = 0x2;

        #endregion

        #region Imports

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, out Point lpPoint);

        [DllImport("dwmapi.dll")]
        private static extern IntPtr DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        #endregion

        #region Structures

        [StructLayout(LayoutKind.Sequential)]
        private struct Margins
        {
            private int left, right, top, bottom;

            public static Margins FromRectangle(Rectangle rectangle)
            {
                var margins = new Margins();
                margins.left = rectangle.Left;
                margins.right = rectangle.Right;
                margins.top = rectangle.Top;
                margins.bottom = rectangle.Bottom;
                return margins;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            private readonly int left, top, right, bottom;

            public Rectangle ToRectangle(Point point)
            {
                return new Rectangle(point.X, point.Y, right - left, bottom - top);
            }
        }

        #endregion
    }
}