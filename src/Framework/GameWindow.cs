using System;
using System.Drawing;
using System.Diagnostics;

using Vector2 = SharpDX.Vector2;

namespace PoeHUD.Framework
{
    public class GameWindow
    {
        private readonly IntPtr handle;

        public GameWindow(Process process)
        {
            Process = process;
            handle = process.MainWindowHandle;
        }

        public Process Process { get; private set; }

        public Rectangle ClientRect()
        {
            return WinApi.GetClientRectangle(handle);
        }

        public bool IsForeground()
        {
            return Imports.GetForegroundWindow() == handle;
        }

        public Vector2 ScreenToClient(int x, int y)
        {
            var point = new Point(x, y);
            WinApi.ScreenToClient(handle, ref point);
            return new Vector2(point.X, point.Y);
        }
    }
}