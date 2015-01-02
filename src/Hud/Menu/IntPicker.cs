using System;

using PoeHUD.Hud.UI;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.Menu
{
    public class IntPicker : MenuItem
    {
        private readonly int min;

        private readonly int max;

        private readonly string text;

        private readonly string settingName;

        private int value;

        private bool isHolding;

        public IntPicker(string text, int min, int max, string settingName)
        {
            this.text = text;
            this.min = min;
            this.max = max;
            value = Settings.GetInt(settingName);
            this.settingName = settingName;
        }

        public override int DesiredWidth
        {
            get { return 210; }
        }

        public override int DesiredHeight
        {
            get { return 30; }
        }

        public override void Render(Graphics graphics)
        {
            if (!IsVisible)
            {
                return;
            }
            Color gray = Color.Gray;
            var textPosition = new Vector2(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 3);
            string textValue = string.Format("{0}: {1}", text, value);
            graphics.DrawText(textValue, 11, textPosition, Color.White, FontDrawFlags.VerticalCenter | FontDrawFlags.Center);
            graphics.DrawBox(Bounds, Color.Black);
            graphics.DrawBox(new RectangleF(Bounds.X + 1, Bounds.Y + 1, Bounds.Width - 2, Bounds.Height - 2), gray);
            graphics.DrawBox(new RectangleF(Bounds.X + 5, Bounds.Y + 3 * Bounds.Height / 4, Bounds.Width - 10, 5), Color.Black);
            float sliderPosition = (Bounds.Width - 10) * (value - min) / (max - min);
            graphics.DrawBox(new RectangleF(Bounds.X + 5 + sliderPosition - 2, Bounds.Y + 3 * Bounds.Height / 4 - 4, 4, 8),
                Color.White);
        }

        protected override void HandleEvent(MouseEventID id, Vector2 pos)
        {
            switch (id)
            {
                case MouseEventID.LeftButtonDown:
                    isHolding = true;
                    break;
                case MouseEventID.LeftButtonUp:
                    CalcValue(pos.X);
                    isHolding = false;
                    break;
                default:
                    if (isHolding && id == MouseEventID.MouseMove)
                    {
                        CalcValue(pos.X);
                    }
                    break;
            }
        }

        protected override bool TestBounds(Vector2 pos)
        {
            return isHolding || base.TestBounds(pos);
        }

        private void CalcValue(float x)
        {
            float num = Bounds.X + 5;
            float num3 = 0;
            if (x > num)
            {
                float num2 = num + Bounds.Width - 10;
                num3 = x >= num2 ? 1 : (x - num) / (num2 - num);
            }
            value = (int)Math.Round(min + num3 * (max - min));
            Settings.SetInt(settingName, value);
        }
    }
}