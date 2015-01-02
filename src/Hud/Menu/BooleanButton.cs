using PoeHUD.Hud.UI;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.Menu
{
    public class BooleanButton : MenuItem
    {
        private readonly string text;

        private readonly string settingName;

        private bool isEnabled;

        public BooleanButton(string text, string settingName)
        {
            this.text = text;
            this.settingName = settingName;
            isEnabled = Settings.GetBool(settingName);
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
            Color color = isEnabled ? Color.Gray : Color.Crimson;
            var textPosition = new Vector2(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2);
            graphics.DrawText(text, 12, textPosition, Color.White, FontDrawFlags.VerticalCenter | FontDrawFlags.Center);
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
                isEnabled = !isEnabled;
                Settings.SetBool(settingName, isEnabled);
            }
        }
    }
}