using System.Windows.Forms;

using SharpDX;

using PointGdi = System.Drawing.Point;

namespace PoeHUD.Framework.InputHooks
{
    public sealed class MouseInfo
    {
        public MouseInfo(MouseButtons buttons, PointGdi position, int wheelDelta)
        {
            Buttons = buttons;
            Position = new Vector2(position.X, position.Y);
            WheelDelta = wheelDelta;
        }

        public MouseButtons Buttons { get; private set; }

        public Vector2 Position { get; private set; }

        public int WheelDelta { get; private set; }

        public bool Handled { get; set; }
    }
}