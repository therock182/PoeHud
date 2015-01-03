using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PoeHUD.Hud
{
    public class MouseHook : IDisposable
    {
        public delegate bool MouseEvent(MouseEventID eventId, int x, int y);

        private readonly int mousehookId;

        private readonly MouseEvent mouseEvent;

        private HookProc LLHookProc;

        public MouseHook(MouseEvent callback)
        {
            mouseEvent = callback;
            LLHookProc = LLMouseProc;

            mousehookId = SetWindowsHookEx(14, LLHookProc, IntPtr.Zero, 0);
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(mousehookId);
        }

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        private int LLMouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                var point = (Point)Marshal.PtrToStructure(lParam, typeof(Point));
                if (mouseEvent((MouseEventID)wParam, point.X, point.Y))
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in mousehook " + ex.Message);
            }
            return CallNextHookEx(mousehookId, nCode, wParam, lParam);
        }

        private delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
    }
}