using System;
using System.Collections.Generic;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.UI;
using PoeHUD.Models;
using PoeHUD.Poe;
using PoeHUD.Poe.Components;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.XpRate
{
    public class XPHRenderer : Plugin
    {
        private RectangleF bounds = new RectangleF(0, 0, 0, 0);
        private string curDisplayString = "0.00 XP/h";
        private string curTimeLeftString = "--h --m --s until level up";
        private bool hasStarted;
        private DateTime lastCalcTime;
        private DateTime startTime;
        private long startXp;

        public RectangleF Bounds
        {
            get
            {
                if (!Settings.GetBool("XphDisplay"))
                {
                    return new RectangleF(0, 0, 0, 0);
                }
                return bounds;
            }
            private set { bounds = value; }
        }


        public XPHRenderer(GameController gameController, Graphics graphics) : base(gameController, graphics)
        {
            GameController.Area.OnAreaChange += CurrentArea_OnAreaChange;
        }

        private void CurrentArea_OnAreaChange(AreaController area)
        {
            startXp = GameController.Player.GetComponent<Player>().XP;
            startTime = DateTime.Now;
            curTimeLeftString = "--h --m --s until level up";
        }

        public override void Render(Dictionary<UiMountPoint, Vector2> mountPoints)
        {
            if (!Settings.GetBool("XphDisplay") ||
                (GameController.Player != null && GameController.Player.GetComponent<Player>().Level >= 100))
            {
                return;
            }
            if (!hasStarted)
            {
                startXp = GameController.Player.GetComponent<Player>().XP;
                startTime = DateTime.Now;
                lastCalcTime = DateTime.Now;
                hasStarted = true;
                return;
            }
            DateTime dtNow = DateTime.Now;
            TimeSpan delta = dtNow - lastCalcTime;

            if (delta.TotalSeconds > 1)
            {
                calculateRemainingExp(dtNow);
                lastCalcTime = dtNow;
            }

            int fontSize = Settings.GetInt("XphDisplay.FontSize");
            int bgAlpha = Settings.GetInt("XphDisplay.BgAlpha");

            var mapWithOffset = mountPoints[UiMountPoint.LeftOfMinimap];

            float yCursor = 0;
            var rateTextSize = Graphics.DrawText(curDisplayString, fontSize, new Vector2(mapWithOffset.X, mapWithOffset.Y),
                Color.White, FontDrawFlags.Right);
            yCursor += rateTextSize.Height;
            var remainingTextSize = Graphics.DrawText(curTimeLeftString, fontSize, new Vector2(mapWithOffset.X, mapWithOffset.Y + yCursor),
                Color.White, FontDrawFlags.Right);
            yCursor += remainingTextSize.Height;
            float thirdLine = mapWithOffset.Y + yCursor;
            var areaLevelNote = Graphics.DrawText(GameController.Area.CurrentArea.DisplayName, fontSize, new Vector2(mapWithOffset.X, thirdLine),
                Color.White, FontDrawFlags.Right);

            string strTimer = AreaInstance.GetTimeString(dtNow - GameController.Area.CurrentArea.TimeEntered);
            var timerSize = Graphics.MeasureText(strTimer, fontSize);
            yCursor += areaLevelNote.Height;

            var clientRect = GameController.Game.IngameState.IngameUi.Map.SmallMinimap.GetClientRect();
            int textWidth =
                Math.Max(Math.Max(rateTextSize.Width, remainingTextSize.Width), areaLevelNote.Width + timerSize.Width + 20) + 10;
            float width = Math.Max(textWidth, Math.Max(clientRect.Width, 0 /*this.overlay.PreloadAlert.Bounds.W*/));
            var rect = new RectangleF(mapWithOffset.X - width + 5, mapWithOffset.Y - 5, width, yCursor + 10);
            Bounds = rect;
            var fps = GameController.Game.IngameState.CurFps;
            Graphics.DrawText(dtNow.ToShortTimeString() + " (" + fps + ")", fontSize, new Vector2(rect.X + 5, mapWithOffset.Y), Color.White);
            Graphics.DrawText(strTimer, fontSize, new Vector2(rect.X + 5, thirdLine), Color.White);

            Graphics.DrawBox(rect, new ColorBGRA(1, 1, 1, (byte)bgAlpha));

            mountPoints[UiMountPoint.LeftOfMinimap] = new Vector2(mapWithOffset.X, mapWithOffset.Y + 5 + rect.Height);
        }

        private void calculateRemainingExp(DateTime dtNow)
        {
            long currentExp = GameController.Player.GetComponent<Player>().XP - startXp;
            var expRate = (float) (currentExp/(dtNow - startTime).TotalHours);
            curDisplayString = (double) expRate > 1000000.0
                ? (expRate/1000000.0).ToString("0.00") + "M XP/h"
                : ((double) expRate > 1000.0
                    ? (expRate/1000.0).ToString("0.00") + "K XP/h"
                    : expRate.ToString("0.00") + " XP/h");
            int level = GameController.Player.GetComponent<Player>().Level;
            if (level < 0 || level + 1 >= Constants.PlayerXpLevels.Length)
            {
                return;
            }
            ulong expRemaining = Constants.PlayerXpLevels[level + 1] -
                                 (ulong) GameController.Player.GetComponent<Player>().XP;
            if (expRate > 1f)
            {
                var num4 = (int) (expRemaining/expRate*3600f);
                int num5 = num4/60;
                int num6 = num5/60;
                curTimeLeftString = string.Concat(num6, "h ", num5%60, "M ", num4%60, "s until level up");
            }
        }
    }
}