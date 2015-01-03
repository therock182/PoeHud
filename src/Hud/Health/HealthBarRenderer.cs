using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.UI;
using PoeHUD.Models;
using PoeHUD.Poe.Components;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.Health
{
    public class HealthBarRenderer : Plugin
    {
        private readonly List<Healthbar>[] healthBars;

        public HealthBarRenderer(GameController gameController, Graphics graphics)
            : base(gameController, graphics)
        {
            healthBars = new List<Healthbar>[Enum.GetValues(typeof(RenderPrio)).Length];
            for (int i = 0; i < healthBars.Length; i++)
            {
                healthBars[i] = new List<Healthbar>();
            }
        }

        public override void Render(Dictionary<UiMountPoint, Vector2> mountPoints)
        {
            if (!GameController.InGame || !Settings.GetBool("Healthbars"))
            {
                return;
            }
            if (!Settings.GetBool("Healthbars.ShowInTown") && GameController.Area.CurrentArea.IsTown)
            {
                return;
            }
            float clientWidth = GameController.Window.ClientRect().W / 2560f;
            float clientHeight = GameController.Window.ClientRect().H / 1600f;

            Parallel.ForEach(healthBars,
                healthbars => healthbars.RemoveAll(x => !(x.Entity.IsValid && x.Entity.IsAlive )));

            foreach (List<Healthbar> healthbars in healthBars)
            {
                var filteredHealthbars = healthbars.AsParallel().Where(x =>x.Show );
                foreach (Healthbar current in filteredHealthbars)
                {
                    Vec3 worldCoords = current.Entity.Pos;
                    Vector2 mobScreenCoords = GameController.Game.IngameState.Camera.WorldToScreen(worldCoords.Translate(0f, 0f, -170f), current.Entity);
                    // System.Diagnostics.Debug.WriteLine("{0} is at {1} => {2} on screen", current.entity.Path, worldCoords, mobScreenCoords);
                    if (mobScreenCoords != new Vector2())
                    {
                        var scaledWidth = (int)(Settings.GetInt(current.Settings + ".Width") * clientWidth);
                        var scaledHeight = (int)(Settings.GetInt(current.Settings + ".Height") * clientHeight);
                        Color color = Settings.GetColor2(current.Settings + ".Color");
                        Color color2 = Settings.GetColor2(current.Settings + ".Outline");
                        Color percentsTextColor = Settings.GetColor2(current.Settings + ".PercentTextColor");
                        var lifeComponent = current.Entity.GetComponent<Life>();
                        float hpPercent = lifeComponent.HPPercentage;
                        float esPercent = lifeComponent.ESPercentage;
                        float hpWidth = hpPercent * scaledWidth;
                        float esWidth = esPercent * scaledWidth;
                        var bg = new RectangleF(mobScreenCoords.X - scaledWidth / 2f, mobScreenCoords.Y - scaledHeight / 2f,
                            scaledWidth,
                            scaledHeight);
                        // Set healthbar color to configured in settings.txt for hostiles when hp is <=10%
                        if (current.Entity.IsHostile && hpPercent <= 0.1)
                        {
                            color = Settings.GetColor2(current.Settings + ".Under10Percent");
                        }

                        // Draw percents or health text for hostiles. Configurable in settings.txt
                        if (current.Entity.IsHostile)
                        {
                            int curHp = lifeComponent.CurHP;
                            int maxHp = lifeComponent.MaxHP;
                            string monsterHp = string.Format("{0}/{1}", ConvertHpToString(curHp), ConvertHpToString(maxHp));
                            string hppercentAsString = Convert.ToString((int)(hpPercent * 100));
                            Color monsterHpColor = (hpPercent <= 0.1)
                                ? Settings.GetColor2(current.Settings + ".HealthTextColorUnder10Percent")
                                : Settings.GetColor2(current.Settings + ".HealthTextColor");

                            if (Settings.GetBool(current.Settings + ".PrintHealthText"))
                            {
                                bg.Y = (int)bg.Y;
                                Size2 size = DrawEntityHealthbarText(monsterHp, bg, monsterHpColor);
                                bg.Y += (size.Height - bg.Height) / 2f; // Correct text in a frame
                            }
                            if (Settings.GetBool(current.Settings + ".PrintPercents"))
                            {
                                DrawEntityHealthPercents(percentsTextColor, hppercentAsString, bg);
                            }
                        }

                        // Draw healthbar
                        DrawEntityHealthbar(color, color2, bg, hpWidth, esWidth);
                    }
                }
            }
        }

        protected override void OnEntityAdded(EntityWrapper entity)
        {
            Healthbar healthbarSettings = new Healthbar(entity);
            if (healthbarSettings.IsValid)
            {
                healthBars[(int)healthbarSettings.Prio].Add(healthbarSettings);
            }
        }

        private static string ConvertHpToString(int hp)
        {
            if (hp < 1000)
            {
                return Convert.ToString(hp);
            }

            return hp < 1000000 ? string.Concat(hp / 1000, "k") : string.Concat(hp / 1000000, "kk");
        }

        
        private void DrawEntityHealthPercents(Color color, string text, RectangleF bg)
        {
            // Draw percents
            Graphics.DrawText(text, 9, new Vector2(bg.X + bg.Width + 4, bg.Y), color);
        }

        private void DrawEntityHealthbar(Color color, Color outline, RectangleF bg, float hpWidth, float esWidth)
        {
            if (outline != Color.Black)
            {
                Graphics.DrawHollowBox(bg, 2f, outline);
            }
            string healthBar = Settings.GetBool("Healthbars.ShowIncrements") ? "healthbar_increment.png" : "healthbar.png";
            Graphics.DrawImage("healthbar_bg.png", bg, color);
            var hpRectangle = new RectangleF(bg.X, bg.Y, hpWidth, bg.Height);
            Graphics.DrawImage(healthBar, hpRectangle, color, hpWidth * 10f / bg.Width);
            if (Settings.GetBool("Healthbars.ShowES"))
            {
                bg.Width = esWidth;
                Graphics.DrawImage("esbar.png", bg);
            }
        }

        private Size2 DrawEntityHealthbarText(string health, RectangleF bg, Color color)
        {
            // Draw monster health ex. "163 / 12k
            return Graphics.DrawText(health, 9, new Vector2(bg.X + bg.Width / 2f, bg.Y), color, FontDrawFlags.Center);
        }
    }
}