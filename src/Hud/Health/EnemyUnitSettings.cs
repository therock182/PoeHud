using PoeHUD.Hud.Settings;

using SharpDX;

namespace PoeHUD.Hud.Health
{
    public class EnemyUnitSettings : AllyUnitSettings
    {
        public EnemyUnitSettings() {}

        public EnemyUnitSettings(uint color, uint outline, uint percentTextColor, bool showText)
            : base(color, outline)
        {
            Under10Percent = Color.FromAbgr(0xffffffff);
            PercentTextColor = Color.FromAbgr(percentTextColor);
            HealthTextColor = Color.FromAbgr(0xffffffff);
            HealthTextColorUnder10Percent = Color.FromAbgr(0xffff00ff);
            ShowPercents = showText;
            ShowHealthText = showText;
        }

        public Color Under10Percent { get; set; }

        public Color PercentTextColor { get; set; }

        public Color HealthTextColor { get; set; }

        public Color HealthTextColorUnder10Percent { get; set; }

        public ToggleNode ShowPercents { get; set; }

        public ToggleNode ShowHealthText { get; set; }
    }
}