using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Framework.Helpers;
using PoeHUD.Hud.UI;
using PoeHUD.Models;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.RemoteMemoryObjects;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.Health
{
    public class HealthBarPlugin : Plugin<HealthBarSettings>
    {
        private readonly List<Healthbar>[] healthBars;

        public HealthBarPlugin(GameController gameController, Graphics graphics, HealthBarSettings settings)
            : base(gameController, graphics, settings)
        {
            healthBars = new List<Healthbar>[Enum.GetValues(typeof(RenderPrio)).Length];
            for (int i = 0; i < healthBars.Length; i++)
            {
                healthBars[i] = new List<Healthbar>();
            }
        }

        public override void Render()
        {
            if (!GameController.InGame || !Settings.Enable || !Settings.ShowInTown && GameController.Area.CurrentArea.IsTown)
            {
                return;
            }

            float clientWidth = GameController.Window.ClientRect().Width / 2560f;
            float clientHeight = GameController.Window.ClientRect().Height / 1600f;

            Parallel.ForEach(healthBars, healthbars => healthbars.RemoveAll(x => !(x.Entity.IsValid && x.Entity.IsAlive)));

            Camera camera = GameController.Game.IngameState.Camera;
            Func<Healthbar, bool> showHealthBar = x => x.IsShow(Settings.ShowEnemies);
            foreach (Healthbar healthbar in healthBars.SelectMany(x => x).AsParallel().AsOrdered().Where(showHealthBar))
            {
                Vector3 worldCoords = healthbar.Entity.Pos;
                Vector2 mobScreenCoords = camera.WorldToScreen(worldCoords.Translate(0f, 0f, -170f), healthbar.Entity);
                // System.Diagnostics.Debug.WriteLine("{0} is at {1} => {2} on screen", current.entity.Path, worldCoords, mobScreenCoords);
                if (mobScreenCoords != new Vector2())
                {
                    float scaledWidth = healthbar.Settings.Width * clientWidth;
                    float scaledHeight = healthbar.Settings.Height * clientHeight;
                    Color color = healthbar.Settings.Color;
                    Color color2 = healthbar.Settings.Outline;
                    var lifeComponent = healthbar.Entity.GetComponent<Life>();
                    float hpPercent = lifeComponent.HPPercentage;
                    float esPercent = lifeComponent.ESPercentage;
                    float hpWidth = hpPercent * scaledWidth;
                    float esWidth = esPercent * scaledWidth;
                    var bg = new RectangleF(mobScreenCoords.X - scaledWidth / 2f, mobScreenCoords.Y - scaledHeight / 2f,
                        scaledWidth,
                        scaledHeight);
                    
                    // Draw percents or health text for hostiles. Configurable in settings.txt
                    if (healthbar.Entity.IsHostile)
                    {
                        var enemySettings = healthbar.Settings as EnemyUnitSettings;

                        // Set healthbar color to configured in settings.txt for hostiles when hp is <=10%
                        if (hpPercent <= 0.1)
                        {
                            color = enemySettings.Under10Percent;
                        }

                        Color percentsTextColor = enemySettings.PercentTextColor;
                        int curHp = lifeComponent.CurHP;
                        int maxHp = lifeComponent.MaxHP;
                        string monsterHp = string.Format("{0}/{1}", ConvertHelper.ToShorten(curHp), ConvertHelper.ToShorten(maxHp));
                        string hppercentAsString = Convert.ToString((int)(hpPercent * 100));
                        Color monsterHpColor = hpPercent <= 0.1
                            ? enemySettings.HealthTextColorUnder10Percent
                            : enemySettings.HealthTextColor;

                        if (enemySettings.ShowHealthText)
                        {
                            bg.Y = (int)bg.Y;
                            Size2 size = DrawEntityHealthbarText(monsterHp, enemySettings.TextSize, bg, monsterHpColor);
                            bg.Y += (size.Height - bg.Height) / 2f; // Correct text in a frame
                        }
                        if (enemySettings.ShowPercents)
                        {
                            DrawEntityHealthPercents(percentsTextColor, enemySettings.TextSize, hppercentAsString, bg);
                        }
                    }

                    // Draw healthbar
                    DrawEntityHealthbar(color, color2, bg, hpWidth, esWidth);
                }
            }
        }

        protected override void OnEntityAdded(EntityWrapper entity)
        {
            var healthbarSettings = new Healthbar(entity, Settings);
            if (healthbarSettings.IsValid)
            {
                healthBars[(int)healthbarSettings.Prio].Add(healthbarSettings);
            }
        }

        private void DrawEntityHealthPercents(Color color, int textSize, string text, RectangleF bg)
        {
            // Draw percents
            Graphics.DrawText(text, textSize, new Vector2(bg.X + bg.Width + 4, bg.Y), color);
        }

        private void DrawEntityHealthbar(Color color, Color outline, RectangleF bg, float hpWidth, float esWidth)
        {
            if (outline != Color.Black)
            {
                Graphics.DrawFrame(bg, 2f, outline);
            }
            string healthBar = Settings.ShowIncrements ? "healthbar_increment.png" : "healthbar.png";
            Graphics.DrawImage("healthbar_bg.png", bg, color);
            var hpRectangle = new RectangleF(bg.X, bg.Y, hpWidth, bg.Height);
            Graphics.DrawImage(healthBar, hpRectangle, color, hpWidth * 10f / bg.Width);
            if (Settings.ShowES)
            {
                bg.Width = esWidth;
                Graphics.DrawImage("esbar.png", bg);
            }
        }

        private Size2 DrawEntityHealthbarText(string health, int textSize, RectangleF bg, Color color)
        {
            // Draw monster health ex. "163 / 12k
            return Graphics.DrawText(health, textSize, new Vector2(bg.X + bg.Width / 2f, bg.Y), color, FontDrawFlags.Center);
        }
    }
}