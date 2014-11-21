using System;
using System.Collections.Generic;
using PoeHUD.Framework;
using PoeHUD.Hud.Interfaces;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.UI;

namespace PoeHUD.Hud.Icons
{
	public class LargeMapRenderer : HudPluginBase
	{
		private readonly Func<IEnumerable<MapIcon>> getIcons;

		public LargeMapRenderer(Func<IEnumerable<MapIcon>> gatherMapIcons)
		{
			getIcons = gatherMapIcons;
		}

		public override void OnEnable()
		{
		}
		public override void OnDisable()
		{
		}

		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			if (!GameController.InGame || !Settings.GetBool("MinimapIcons"))
			{
				return;
			}
			bool largeMapVisible = GameController.Game.IngameState.IngameUi.Minimap.OrangeWords.IsVisible;
			if (!largeMapVisible)
				return;

			var camera = GameController.Game.IngameState.Camera;
			BigMinimap mapWindow = GameController.Game.IngameState.IngameUi.Minimap;
			Rect rcMap = mapWindow.GetClientRect();

			Vec2 playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
			float pPosZ = GameController.Player.GetComponent<Render>().Z;
			Vec2 screenCenter = new Vec2(rcMap.W / 2, rcMap.H / 2) + new Vec2(rcMap.X, rcMap.Y);
			float diag = (float)Math.Sqrt(camera.Width * camera.Width + camera.Height * camera.Height);

			const float scale = 1280f;

			foreach(MapIcon icon in getIcons())
			{
				if (icon.ShouldSkip())
					continue;

				float iZ = icon.Entity.GetComponent<Render>().Z;
				Vec2 point = screenCenter + MapIcon.deltaInWorldToMinimapDelta(icon.WorldPosition - playerPos, diag, scale, (int)((iZ - pPosZ)/ 10));

				var texture = icon.LargeMapIcon ?? icon.MinimapIcon;
				int size = icon.SizeOfLargeIcon.GetValueOrDefault(icon.Size * 2);
				Rect rect = new Rect(point.X - size / 2, point.Y - size / 2, size, size);
				texture.DrawAt(rc, point, rect);
			}
		}
	}
}
