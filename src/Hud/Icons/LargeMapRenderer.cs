using System;
using System.Collections.Generic;
using System.Threading;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.Interfaces;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.UI;
using PoeHUD.Poe.UI.Elements;
using Map = PoeHUD.Poe.UI.Elements.Map;

namespace PoeHUD.Hud.Icons
{
    public class LargeMapRenderer : HudPluginBase
    {
        private readonly Func<IEnumerable<MapIcon>> getIcons;

        public LargeMapRenderer(GameController gameController, Func<IEnumerable<MapIcon>> gatherMapIcons)
            : base(gameController)
        {
            getIcons = gatherMapIcons;
        }


        public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
        {
            if (!GameController.InGame || !Settings.GetBool("MinimapIcons"))
            {
                return;
            }
            bool largeMapVisible = GameController.Game.IngameState.IngameUi.Map.OrangeWords.IsVisible;
            if (!largeMapVisible)
                return;

            var camera = GameController.Game.IngameState.Camera;
            Map mapWindow = GameController.Game.IngameState.IngameUi.Map;
            Rect rcMap = mapWindow.GetClientRect();

            Vec2 playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
            float pPosZ = GameController.Player.GetComponent<Render>().Z;
            Vec2 screenCenter = new Vec2(rcMap.W / 2, rcMap.H / 2) + new Vec2(rcMap.X, rcMap.Y) + new Vec2((int)mapWindow.ShiftX, (int)mapWindow.ShiftY);
            float diag = (float)Math.Sqrt(camera.Width * camera.Width + camera.Height * camera.Height);
            var k = camera.Width < 1024 ? 1120 : 1024;
            float scale = (float)k / camera.Height * camera.Width * 3 / 4;

            foreach (MapIcon icon in getIcons())
            {
                if (icon.ShouldSkip())
                    continue;

                float iZ = icon.EntityWrapper.GetComponent<Render>().Z;
                Vec2 point = screenCenter + MapIcon.deltaInWorldToMinimapDelta(icon.WorldPosition - playerPos, diag, scale, (int)((iZ - pPosZ) / 20));

                var texture = icon.LargeMapIcon ?? icon.MinimapIcon;
                int size = icon.SizeOfLargeIcon.GetValueOrDefault(icon.Size * 2);
                Rect rect = new Rect(point.X - size / 2, point.Y - size / 2, size, size);
                texture.DrawAt(rc, point, rect);
            }
        }
    }
}
