using System.Collections.Generic;
using PoeHUD.Framework;
using PoeHUD.Hud.UI;

using SharpDX;

namespace PoeHUD.Hud.Menu
{
	public abstract class MenuItem
	{
		protected List<MenuItem> children;
		protected MenuItem currentHover;
		protected bool isVisible;
		public RectangleF Bounds
		{
			get;
			set;
		}
		public abstract int DesiredWidth
		{
			get;
		}
		public abstract int DesiredHeight
		{
			get;
		}
		public MenuItem()
		{
			this.children = new List<MenuItem>();
		}
		public abstract void Render(Graphics graphics);
		protected abstract void HandleEvent(MouseEventID id, Vector2 pos);
		protected virtual bool TestBounds(Vector2 pos)
		{
			return this.Bounds.Contains(pos);
		}
		public bool TestHit(Vector2 pos)
		{
			if (!this.isVisible)
			{
				return false;
			}
			if (this.TestBounds(pos))
			{
				return true;
			}
			foreach (MenuItem current in this.children)
			{
				if (current.TestHit(pos))
				{
					return true;
				}
			}
			return false;
		}
		public void SetVisible(bool visible)
		{
			this.isVisible = visible;
			if (!visible)
			{
				foreach (MenuItem current in this.children)
				{
					current.SetVisible(false);
				}
			}
		}
		public void SetHovered(bool hover)
		{
			foreach (MenuItem current in this.children)
			{
				current.SetVisible(hover);
			}
		}
		public void OnEvent(MouseEventID id, Vector2 pos)
		{
			if (id == MouseEventID.MouseMove)
			{
				if (this.TestBounds(pos))
				{
					this.HandleEvent(id, pos);
					if (this.currentHover != null)
					{
						this.currentHover.SetHovered(false);
						this.currentHover = null;
					}
					return;
				}
				if (this.currentHover != null)
				{
					if (this.currentHover.TestHit(pos))
					{
						this.currentHover.OnEvent(id, pos);
						return;
					}
					this.currentHover.SetHovered(false);
				}
				MenuItem childAt = this.GetChildAt(pos);
				if (childAt != null)
				{
					childAt.SetHovered(true);
					this.currentHover = childAt;
					return;
				}
				this.currentHover = null;
				return;
			}
			else
			{
				if (this.TestBounds(pos))
				{
					this.HandleEvent(id, pos);
					return;
				}
				if (this.currentHover != null)
				{
					this.currentHover.OnEvent(id, pos);
				}
				return;
			}
		}
		private MenuItem GetChildAt(Vector2 pos)
		{
			foreach (MenuItem current in this.children)
			{
				if (current.TestHit(pos))
				{
					return current;
				}
			}
			return null;
		}
		public void AddChild(MenuItem item)
		{
			float num = this.Bounds.Y;
			float x = this.Bounds.X + this.Bounds.Width;
			foreach (MenuItem current in this.children)
			{
				num += current.Bounds.Height;
			}
			item.Bounds = new RectangleF(x, num, item.DesiredWidth, item.DesiredHeight);
			this.children.Add(item);
		}
	}
}
