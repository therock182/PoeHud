using PoeHUD.Hud.Settings;
using PoeHUD.Hud.UI;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.Menu
{
    public class ToggleButton : MenuItem
    {
        private readonly string name;

        private readonly ToggleNode node;

        public ToggleButton(string name, ToggleNode node)
        {
            this.name = name;
            this.node = node;
        }

        public override int DesiredHeight
        {
            get { return 25; }
        }

        public override int DesiredWidth
        {
            get { return 210; }
        }

        public override void Render(Graphics graphics)
        {
            if (!IsVisible)
            {
                return;
            }
            Color color = node.Value ? Color.Gray : Color.Crimson;
            var textPosition = new Vector2(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2);
            graphics.DrawText(name, 12, textPosition, Color.White, FontDrawFlags.VerticalCenter | FontDrawFlags.Center);
            graphics.DrawBox(Bounds, Color.Black);
            graphics.DrawBox(new RectangleF(Bounds.X + 1, Bounds.Y + 1, Bounds.Width - 2, Bounds.Height - 2), color);
            if (Children.Count > 0)
            {
                float num = (Bounds.Width - 2) * 0.05f;
                float num2 = (Bounds.Height - 2) / 2;
                var imgRect = new RectangleF(Bounds.X + Bounds.Width - 1 - num, Bounds.Y + 1 + num2 - num2 / 2, num, num2);
                graphics.DrawImage("menu_submenu.png", imgRect);
            }
            Children.ForEach(x => x.Render(graphics));
        }

        protected override void HandleEvent(MouseEventID id, Vector2 pos)
        {
            if (id == MouseEventID.LeftButtonDown)
            {
                node.Value = !node.Value;
            }
        }
    }
}