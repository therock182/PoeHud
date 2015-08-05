using System.Linq;
using PoeHUD.Framework.Helpers;
using PoeHUD.Hud.UI;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.Menu
{
    public sealed class RootButton : MenuItem
    {
        private MenuItem currentHover;

        private bool visible;

        public RootButton(Vector2 position)
        {
            Bounds = new RectangleF(position.X, position.Y, DesiredWidth, DesiredHeight);
        }

        public override int DesiredWidth
        {
            get { return 80; }
        }

        public override int DesiredHeight
        {
            get { return 24; }
        }

        public override void AddChild(MenuItem item)
        {
            base.AddChild(item);
            float x = item.Bounds.X - DesiredWidth;
            float y = item.Bounds.Y + DesiredHeight;
            item.Bounds = new RectangleF(x, y, item.Bounds.Width, item.Bounds.Height);
        }

        public bool OnMouseEvent(MouseEventID id, Vector2 pos)
        {
            if (currentHover != null && currentHover.TestHit(pos))
            {
                currentHover.OnEvent(id, pos);
                return id != MouseEventID.MouseMove;
            }

            if (id == MouseEventID.MouseMove)
            {
                MenuItem button = Children.FirstOrDefault(b => b.TestHit(pos));
                if (button != null)
                {
                    if (currentHover != null)
                    {
                        currentHover.SetHovered(false);
                    }
                    currentHover = button;
                    button.SetHovered(true);
                }
                return false;
            }

            if (Bounds.Contains(pos) && id == MouseEventID.LeftButtonDown)
            {
                visible = !visible;
                currentHover = null;
                Children.ForEach(button => button.SetVisible(visible));
                return true;
            }

            return false;
        }

        public override void Render(Graphics graphics)
        {
            // TODO move to settings
            Color boxColor = Color.Gray;
            boxColor.A = 100;
            graphics.DrawBox(new RectangleF(Bounds.X, Bounds.Y, DesiredWidth, DesiredHeight), boxColor);
            var textColor = new ColorBGRA(255, 255, 255, 200);
            graphics.DrawText("Menu [F12]", 15, Bounds.TopLeft.Translate(40, 12), textColor, FontDrawFlags.VerticalCenter | FontDrawFlags.Center);
            Children.ForEach(x => x.Render(graphics));
        }

        protected override void HandleEvent(MouseEventID id, Vector2 pos) {}
    }
}