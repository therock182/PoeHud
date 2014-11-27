using System;
using System.Collections.Generic;
using System.Drawing;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Game;
using PoeHUD.Poe.EntityComponents;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.XpRate
{
    public class XPHRenderer : HudPluginBase
    {
        private Rect bounds = new Rect(0, 0, 0, 0);
        private string curDisplayString = "0.00 XP/h";
        private string curTimeLeftString = "--h --m --s until level up";
        private bool hasStarted;
        private DateTime lastCalcTime;
        private DateTime startTime;
        private long startXp;

        public Rect Bounds
        {
            get
            {
                if (!Settings.GetBool("XphDisplay"))
                {
                    return new Rect(0, 0, 0, 0);
                }
                return bounds;
            }
            private set { bounds = value; }
        }


        public XPHRenderer(GameController gameController) : base(gameController)
        {
            GameController.Area.OnAreaChange += CurrentArea_OnAreaChange;
        }

        private void CurrentArea_OnAreaChange(AreaController area)
        {
            startXp = GameController.Player.GetComponent<Player>().XP;
            startTime = DateTime.Now;
            curTimeLeftString = "--h --m --s until level up";
        }

        public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
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

            Vec2 mapWithOffset = mountPoints[UiMountPoint.LeftOfMinimap];

            int yCursor = 0;
            Vec2 rateTextSize = rc.AddTextWithHeight(new Vec2(mapWithOffset.X, mapWithOffset.Y), curDisplayString,
                Color.White, fontSize, DrawTextFormat.Right);
            yCursor += rateTextSize.Y;
            Vec2 remainingTextSize = rc.AddTextWithHeight(new Vec2(mapWithOffset.X, mapWithOffset.Y + yCursor),
                curTimeLeftString, Color.White, fontSize, DrawTextFormat.Right);
            yCursor += remainingTextSize.Y;
            int thirdLine = mapWithOffset.Y + yCursor;
            Vec2 areaLevelNote = rc.AddTextWithHeight(new Vec2(mapWithOffset.X, thirdLine),
                GameController.Area.CurrentArea.DisplayName, Color.White, fontSize, DrawTextFormat.Right);

            string strTimer = AreaInstance.GetTimeString(dtNow - GameController.Area.CurrentArea.TimeEntered);
            Vec2 timerSize = rc.MeasureString(strTimer, fontSize, DrawTextFormat.Left);
            yCursor += areaLevelNote.Y;

            Rect clientRect = GameController.Game.IngameState.IngameUi.Minimap.SmallMinimap.GetClientRect();
            int textWidth =
                Math.Max(Math.Max(rateTextSize.X, remainingTextSize.X), areaLevelNote.X + timerSize.X + 20) + 10;
            int width = Math.Max(textWidth, Math.Max(clientRect.W, 0 /*this.overlay.PreloadAlert.Bounds.W*/));
            var rect = new Rect(mapWithOffset.X - width + 5, mapWithOffset.Y - 5, width, yCursor + 10);
            Bounds = rect;

            rc.AddTextWithHeight(new Vec2(rect.X + 5, mapWithOffset.Y), dtNow.ToShortTimeString(), Color.White, fontSize,
                DrawTextFormat.Left);
            rc.AddTextWithHeight(new Vec2(rect.X + 5, thirdLine), strTimer, Color.White, fontSize, DrawTextFormat.Left);

            rc.AddBox(rect, Color.FromArgb(bgAlpha, 1, 1, 1));

            mountPoints[UiMountPoint.LeftOfMinimap] = new Vec2(mapWithOffset.X, mapWithOffset.Y + 5 + rect.H);
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