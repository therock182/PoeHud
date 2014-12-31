using PoeHUD.Framework;
using PoeHUD.Hud.UI;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.Menu
{
	public class BooleanButton : MenuItem
	{
		private string text;
		private string settingName;
		public bool isEnabled;
		public override int DesiredHeight
		{
			get
			{
				return 25;
			}
		}
		public override int DesiredWidth
		{
			get
			{
				return 210;
			}
		}
		public BooleanButton(string text, string settingName)
		{
			this.text = text;
			this.settingName = settingName;
			this.isEnabled = Settings.GetBool(settingName);
		}
		public override void Render(Graphics graphics)
		{
			if (!this.isVisible)
			{
				return;
			}
			Color color = this.isEnabled ? Color.Gray : Color.Crimson;
            graphics.DrawText(text, 12, new Vector2(base.Bounds.X + base.Bounds.Width / 2, base.Bounds.Y + base.Bounds.Height / 2), Color.White, FontDrawFlags.VerticalCenter | FontDrawFlags.Center);
            graphics.DrawBox(base.Bounds, Color.Black);
            graphics.DrawBox(new RectangleF(base.Bounds.X + 1, base.Bounds.Y + 1, base.Bounds.Width - 2, base.Bounds.Height - 2), color);
			if (this.children.Count > 0)
			{
				float num = ((float)(base.Bounds.Width - 2) * 0.05f);
				float num2 = (base.Bounds.Height - 2) / 2;
				graphics.DrawImage("menu_submenu.png", new RectangleF(base.Bounds.X + base.Bounds.Width - 1 - num, base.Bounds.Y + 1 + num2 - num2 / 2, num, num2));
			}
			foreach (MenuItem current in this.children)
			{
				current.Render(graphics);
			}
		}
		protected override void HandleEvent(MouseEventID id, Vector2 pos)
		{
			if (id == MouseEventID.LeftButtonDown)
			{
				this.isEnabled = !this.isEnabled;
			}
			Settings.SetBool(this.settingName, this.isEnabled);
		}
	}
}
