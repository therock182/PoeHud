using System;
using System.Collections.Generic;
using PoeHUD.Framework;
using PoeHUD.Hud.Interfaces;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.UI;

namespace PoeHUD.Hud.Icons
{
	public class MinimapRenderer : HudPluginBase
	{
		private readonly Func<IEnumerable<MapIcon>> getIcons;
		

		public MinimapRenderer(Func<IEnumerable<MapIcon>> gatherMapIcons)
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
			Element smallMinimap = GameController.Game.IngameState.IngameUi.Minimap.SmallMinimap;
			if( !smallMinimap.IsVisible )
				return;


			Vec2 playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
			float pPosZ = GameController.Player.GetComponent<Render>().Z;
			
			const float scale = 240f;
			Rect clientRect = smallMinimap.GetClientRect();
			Vec2 minimapCenter = new Vec2(clientRect.X + clientRect.W / 2, clientRect.Y + clientRect.H / 2);
			double diag = Math.Sqrt(clientRect.W * clientRect.W + clientRect.H * clientRect.H) / 2.0;
			foreach(MapIcon icon in getIcons())
			{
				if (icon.ShouldSkip())
					continue;

				float iZ = icon.Entity.GetComponent<Render>().Z;
				Vec2 point = minimapCenter + MapIcon.deltaInWorldToMinimapDelta(icon.WorldPosition - playerPos, diag, scale, (int)((iZ - pPosZ) / 20));

				var texture = icon.MinimapIcon;
				int size = icon.Size;
				Rect rect = new Rect(point.X - size / 2, point.Y - size / 2, size, size);
				texture.DrawAt(rc, point, rect);
			}
		}
	}
}
