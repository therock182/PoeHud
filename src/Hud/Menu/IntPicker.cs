using System;
using PoeHUD.Framework;
using PoeHUD.Hud.UI;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.Menu
{
	public class IntPicker : MenuItem
	{
		private int value;
		private int min;
		private int max;
		private string text;
		private bool isHolding;
		private string settingName;
		public override int DesiredWidth
		{
			get
			{
				return 210;
			}
		}
		public override int DesiredHeight
		{
			get
			{
				return 30;
			}
		}
		public IntPicker(string text, int min, int max, string settingName)
		{
			this.text = text;
			this.min = min;
			this.max = max;
			this.value = Settings.GetInt(settingName);
			this.settingName = settingName;
		}
		public override void Render(Graphics graphics)
		{
			if (!this.isVisible)
			{
				return;
			}
			Color gray = Color.Gray;
            graphics.DrawText(this.text + ": " + this.value, 11, new Vector2(base.Bounds.X + base.Bounds.Width / 2, base.Bounds.Y + base.Bounds.Height / 3), Color.White, FontDrawFlags.VerticalCenter | FontDrawFlags.Center);
            graphics.DrawBox(base.Bounds, Color.Black);
            graphics.DrawBox(new RectangleF(base.Bounds.X + 1, base.Bounds.Y + 1, base.Bounds.Width - 2, base.Bounds.Height - 2), gray);
            graphics.DrawBox(new RectangleF(base.Bounds.X + 5, base.Bounds.Y + 3 * base.Bounds.Height / 4, base.Bounds.Width - 10, 5), Color.Black);
			float num = (float)(this.value - this.min) / (float)(this.max - this.min);
			int num2 = (int)((float)(base.Bounds.Width - 10) * num);
            graphics.DrawBox(new RectangleF(base.Bounds.X + 5 + num2 - 2, base.Bounds.Y + 3 * base.Bounds.Height / 4 - 4, 4, 8), Color.White);
		}
		private void CalcValue(float x)
		{
			float num = base.Bounds.X + 5;
			float num2 = num + base.Bounds.Width - 10;
			float num3;
			if (x <= num)
			{
				num3 = 0f;
			}
			else
			{
				if (x >= num2)
				{
					num3 = 1f;
				}
				else
				{
					num3 = (float)(x - num) / (float)(num2 - num);
				}
			}
			this.value = (int)Math.Round((this.min + num3 * (this.max - this.min)));
			Settings.SetInt(this.settingName, this.value);
		}
		protected override bool TestBounds(Vector2 pos)
		{
			return this.isHolding || base.TestBounds(pos);
		}
		protected override void HandleEvent(MouseEventID id, Vector2 pos)
		{
			if (id == MouseEventID.LeftButtonDown)
			{
				this.isHolding = true;
				return;
			}
			if (id == MouseEventID.LeftButtonUp)
			{
				this.CalcValue(pos.X);
				this.isHolding = false;
				return;
			}
			if (this.isHolding && id == MouseEventID.MouseMove)
			{
				this.CalcValue(pos.X);
			}
		}
	}
}
