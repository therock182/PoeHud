using System;
using System.Collections.Generic;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.Interfaces;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.UI;

namespace PoeHUD.Hud.Icons
{
	public class MinimapRenderer : HudPluginBase
	{
		private readonly Func<IEnumerable<MapIcon>> getIcons;


        public MinimapRenderer(GameController gameController, Func<IEnumerable<MapIcon>> gatherMapIcons): base(gameController)
	    {
            getIcons = gatherMapIcons;
	    }

	
		public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			if (!GameController.InGame || !Settings.GetBool("MinimapIcons"))
			{
				return;
			}
			Element smallMinimap = GameController.Game.IngameState.IngameUi.Map.SmallMinimap;
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

				float iZ = icon.EntityWrapper.GetComponent<Render>().Z;
				Vec2 point = minimapCenter + MapIcon.deltaInWorldToMinimapDelta(icon.WorldPosition - playerPos, diag, scale, (int)((iZ - pPosZ) / 20));

				var texture = icon.MinimapIcon;
				int size = icon.Size;
				Rect rect = new Rect(point.X - size / 2, point.Y - size / 2, size, size);
				texture.DrawAt(rc, point, rect);
			}
		}
	}
}
