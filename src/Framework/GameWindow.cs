using System;
using System.Diagnostics;

using SharpDX;

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

        public Rect ClientRect()
        {
            Rect result;
            Imports.GetClientRect(handle, out result);
            Vec2 vec = Vec2.Empty;
            Imports.ClientToScreen(handle, ref vec);
            result.X = vec.X;
            result.Y = vec.Y;
            return result;
        }

        public bool IsForeground()
        {
            return Imports.GetForegroundWindow() == handle;
        }

        public Vector2 ScreenToClient(int x, int y)
        {
            var vector = new Vec2(x, y);
            Imports.ScreenToClient(handle, ref vector);
            return new Vector2(vector.X, vector.Y);
        }
    }
}