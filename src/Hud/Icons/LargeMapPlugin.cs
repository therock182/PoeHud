using System;
using System.Collections.Generic;
using System.Linq;

using PoeHUD.Controllers;
using PoeHUD.Hud.UI;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.RemoteMemoryObjects;

using SharpDX;

using Map = PoeHUD.Poe.UI.Elements.Map;

namespace PoeHUD.Hud.Icons
{
    public class LargeMapPlugin : Plugin<MapIconsSettings>
    {
        private readonly Func<IEnumerable<MapIcon>> getIcons;

        public LargeMapPlugin(GameController gameController, Graphics graphics, Func<IEnumerable<MapIcon>> gatherMapIcons,
            MapIconsSettings settings)
            : base(gameController, graphics, settings)
        {
            getIcons = gatherMapIcons;
        }

        public override void Render()
        {
            if (!Settings.Enable || !GameController.InGame || !Settings.IconsOnLargeMap
                || !GameController.Game.IngameState.IngameUi.Map.OrangeWords.IsVisible)
            {
                return;
            }

            Camera camera = GameController.Game.IngameState.Camera;
            Map mapWindow = GameController.Game.IngameState.IngameUi.Map;
            RectangleF rcMap = mapWindow.GetClientRect();

            Vector2 playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
            float pPosZ = GameController.Player.GetComponent<Render>().Z;
            Vector2 screenCenter = new Vector2(rcMap.Width / 2, rcMap.Height / 2) + new Vector2(rcMap.X, rcMap.Y)
                + new Vector2(mapWindow.ShiftX, mapWindow.ShiftY);
            var diag = (float)Math.Sqrt(camera.Width * camera.Width + camera.Height * camera.Height);
            float k = camera.Width < 1024f ? 1120f : 1024f;
            float scale = k / camera.Height * camera.Width * 3 / 4;

            foreach (MapIcon icon in getIcons().Where(x => x.IsVisible()))
            {
                float iZ = icon.EntityWrapper.GetComponent<Render>().Z;
                Vector2 point = screenCenter
                    + MapIcon.deltaInWorldToMinimapDelta(icon.WorldPosition - playerPos, diag, scale, (iZ - pPosZ) / 20);

                HudTexture texture = icon.LargeMapIcon ?? icon.MinimapIcon;
                int size = icon.SizeOfLargeIcon.GetValueOrDefault(icon.Size * 2);
                var rect = new RectangleF(point.X - size / 2f, point.Y - size / 2f, size, size);
                texture.Draw(Graphics, rect);
            }
        }
    }
}