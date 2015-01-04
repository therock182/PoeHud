using System;
using System.Collections.Generic;
using System.Linq;

using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.Interfaces;
using PoeHUD.Hud.UI;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.UI;

using SharpDX;

namespace PoeHUD.Hud.Icons
{
	public class MinimapPlugin : Plugin<MapIconsSettings>
	{
		private readonly Func<IEnumerable<MapIcon>> getIcons;


        public MinimapPlugin(GameController gameController, Graphics graphics, Func<IEnumerable<MapIcon>> gatherMapIcons, MapIconsSettings settings)
            : base(gameController, graphics, settings)
	    {
            getIcons = gatherMapIcons;
	    }

	
		public override void Render(Dictionary<UiMountPoint, Vector2> mountPoints)
		{
			if (!GameController.InGame || !Settings.Enable || !Settings.IconsOnMinimap)
			{
				return;
			}
			Element smallMinimap = GameController.Game.IngameState.IngameUi.Map.SmallMinimap;
			if( !smallMinimap.IsVisible )
				return;


			Vec2 playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
			float pPosZ = GameController.Player.GetComponent<Render>().Z;
			
			const float scale = 240f;
			var clientRect = smallMinimap.GetClientRect();
			var minimapCenter = new Vector2(clientRect.X + clientRect.Width / 2, clientRect.Y + clientRect.Height / 2);
			double diag = Math.Sqrt(clientRect.Width * clientRect.Width + clientRect.Height * clientRect.Height) / 2.0;
			foreach(MapIcon icon in getIcons().Where(x => x.IsVisible()))
			{
				float iZ = icon.EntityWrapper.GetComponent<Render>().Z;
				var point = minimapCenter + MapIcon.deltaInWorldToMinimapDelta(icon.WorldPosition - playerPos, diag, scale, (iZ - pPosZ) / 20);

				var texture = icon.MinimapIcon;
				int size = icon.Size;
				var rect = new RectangleF(point.X - size / 2f, point.Y - size / 2f, size, size);
				texture.Draw(Graphics, rect);
			}
		}
	}
}
