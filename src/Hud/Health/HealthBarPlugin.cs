using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PoeHUD.Controllers;
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
        private readonly Dictionary<CreatureType, List<HealthBar>> healthBars;

        public HealthBarPlugin(GameController gameController, Graphics graphics, HealthBarSettings settings)
            : base(gameController, graphics, settings)
        {
            CreatureType[] types = Enum.GetValues(typeof(CreatureType)).Cast<CreatureType>().ToArray();
            healthBars = new Dictionary<CreatureType, List<HealthBar>>(types.Length);
            foreach (CreatureType type in types)
            {
                healthBars.Add(type, new List<HealthBar>());
            }
        }

        public override void Render()
        {
            if (!Settings.Enable || !GameController.InGame || !Settings.ShowInTown && GameController.Area.CurrentArea.IsTown)
            {
                return;
            }

            RectangleF windowRectangle = GameController.Window.GetWindowRectangle();
            var windowSize = new Size2F(windowRectangle.Width / 2560, windowRectangle.Height / 1600);

            Camera camera = GameController.Game.IngameState.Camera;
            Func<HealthBar, bool> showHealthBar = x => x.IsShow(Settings.ShowEnemies);
            Parallel.ForEach(healthBars, x => x.Value.RemoveAll(hp => !(hp.Entity.IsValid && hp.Entity.IsAlive)));
            foreach (HealthBar healthBar in healthBars.SelectMany(x => x.Value).AsParallel().AsOrdered().Where(showHealthBar))
            {
                Vector3 worldCoords = healthBar.Entity.Pos;
                Vector2 mobScreenCoords = camera.WorldToScreen(worldCoords.Translate(0, 0, -170), healthBar.Entity);
                if (mobScreenCoords != new Vector2())
                {
                    DrawHealthBar(healthBar, windowSize, mobScreenCoords);
                }
            }
        }

        protected override void OnEntityAdded(EntityWrapper entity)
        {
            var healthbarSettings = new HealthBar(entity, Settings);
            if (healthbarSettings.IsValid)
            {
                healthBars[healthbarSettings.Type].Add(healthbarSettings);
            }
        }

        private void DrawBackground(Color color, Color outline, RectangleF bg, float hpWidth, float esWidth)
        {
            if (outline != Color.Black)
            {
                Graphics.DrawFrame(bg, 2, outline);
            }
            string healthBar = Settings.ShowIncrements ? "healthbar_increment.png" : "healthbar.png";
            Graphics.DrawImage("healthbar_bg.png", bg, color);
            var hpRectangle = new RectangleF(bg.X, bg.Y, hpWidth, bg.Height);
            Graphics.DrawImage(healthBar, hpRectangle, color, hpWidth * 10 / bg.Width);
            if (Settings.ShowES)
            {
                bg.Width = esWidth;
                Graphics.DrawImage("esbar.png", bg);
            }
        }

        private void DrawHealthBar(HealthBar healthBar, Size2F windowSize, Vector2 coords)
        {
            float scaledWidth = healthBar.Settings.Width * windowSize.Width;
            float scaledHeight = healthBar.Settings.Height * windowSize.Height;
            Color color = healthBar.Settings.Color;
            var life = healthBar.Entity.GetComponent<Life>();
            float hpPercent = life.HPPercentage;
            float esPercent = life.ESPercentage;
            float hpWidth = hpPercent * scaledWidth;
            float esWidth = esPercent * scaledWidth;

            var bg = new RectangleF(coords.X - scaledWidth / 2, coords.Y - scaledHeight / 2, scaledWidth, scaledHeight);
            if (healthBar.Entity.IsHostile)
            {
                var enemySettings = healthBar.Settings as EnemyUnitSettings;
                if (hpPercent <= 0.1f)
                {
                    color = enemySettings.Under10Percent;
                }

                bg.Y = DrawHp(life, hpPercent, enemySettings, bg);
                DrawPercents(enemySettings, hpPercent, bg);
            }

            DrawBackground(color, healthBar.Settings.Outline, bg, hpWidth, esWidth);
        }

        private float DrawHp(Life life, float hpPercent, EnemyUnitSettings enemySettings, RectangleF bg)
        {
            if (enemySettings.ShowHealthText)
            {
                string curHp = ConvertHelper.ToShorten(life.CurHP);
                string maxHp = ConvertHelper.ToShorten(life.MaxHP);
                string text = string.Format("{0}/{1}", curHp, maxHp);
                Color color = hpPercent <= 0.1f ? enemySettings.HealthTextColorUnder10Percent : enemySettings.HealthTextColor;
                var position = new Vector2(bg.X + bg.Width / 2, bg.Y);
                Size2 size = Graphics.DrawText(text, enemySettings.TextSize, position, color, FontDrawFlags.Center);
                return (int)bg.Y + (size.Height - bg.Height) / 2;
            }
            return bg.Y;
        }

        private void DrawPercents(EnemyUnitSettings settings, float hpPercent, RectangleF bg)
        {
            if (settings.ShowPercents)
            {
                string text = Convert.ToString((int)(hpPercent * 100));
                var position = new Vector2(bg.X + bg.Width + 4, bg.Y);
                Graphics.DrawText(text, settings.TextSize, position, settings.PercentTextColor);
            }
        }
    }
}