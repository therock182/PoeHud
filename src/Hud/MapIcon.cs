using System;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Models;
using PoeHUD.Poe.Components;

using SharpDX;

namespace PoeHUD.Hud
{
	public class MapIconCreature : MapIcon
	{
		public MapIconCreature(EntityWrapper entityWrapper) : base(entityWrapper) { }
		public MapIconCreature(EntityWrapper entityWrapper, HudTexture hudTexture, int iconSize) : base(entityWrapper, hudTexture, iconSize) { }

		public override bool ShouldSkip() { return !EntityWrapper.IsAlive; }
	}

	public class MapIconChest : MapIcon
	{
		public MapIconChest(EntityWrapper entityWrapper) : base(entityWrapper) { }
		public MapIconChest(EntityWrapper entityWrapper, HudTexture hudTexture, int iconSize) : base(entityWrapper, hudTexture, iconSize) { }

		public override bool IsEntityStillValid()
		{
		    return EntityWrapper.IsValid && !EntityWrapper.GetComponent<Chest>().IsOpened;
		}
	}

	// Settings.GetBool("MinimapIcons.Masters");
	// Settings.GetBool("MinimapIcons.AlertedItems");
	// Settings.GetBool("MinimapIcons.Monsters")
	// Settings.GetBool("MinimapIcons.Minions")
	// Settings.GetBool("MinimapIcons.Chests");
	// Settings.GetBool("MinimapIcons.Strongboxes");


	public class MapIcon
	{
		public readonly EntityWrapper EntityWrapper;
		public HudTexture MinimapIcon;
		public HudTexture LargeMapIcon;
		public int Size;
		public int? SizeOfLargeIcon;

		public Vec2 WorldPosition { get { return EntityWrapper.GetComponent<Positioned>().GridPos; } }

		public MapIcon(EntityWrapper entityWrapper) {
			EntityWrapper = entityWrapper;
		}

		public MapIcon(EntityWrapper entityWrapper, HudTexture hudTexture, int iconSize = 10) : this(entityWrapper)
		{
			MinimapIcon = hudTexture;
			Size = iconSize;
		}

		public static Vector2 deltaInWorldToMinimapDelta(Vec2 delta, double diag, float scale, float deltaZ = 0)
		{
			const double CameraAngle = Math.PI / 180 * 38;

			// Values according to 40 degree rotation of cartesian coordiantes, still doesn't seem right but closer
            float cosX = (float)(delta.X / scale * diag * Math.Cos(CameraAngle));
            float cosY = (float)(delta.Y / scale * diag * Math.Cos(CameraAngle));
            float sinX = (float)(delta.X / scale * diag * Math.Sin(CameraAngle));
            float sinY = (float)(delta.Y / scale * diag * Math.Sin(CameraAngle));
			// 2D rotation formulas not correct, but it's what appears to work?
			return new Vector2(cosX - cosY, -sinX - sinY + deltaZ);
		}

		public virtual bool IsEntityStillValid() { return EntityWrapper.IsValid; }
		public virtual bool ShouldSkip() { return false; }
	}
}
