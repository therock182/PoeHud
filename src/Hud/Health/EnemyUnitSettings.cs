using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.Health
{
    public class EnemyUnitSettings : AllyUnitSettings
    {
        public EnemyUnitSettings() {}

        public EnemyUnitSettings(uint color, uint outline, uint percentTextColor, bool showText)
            : base(color, outline)
        {
            Under10Percent = 0xffffffff;
            PercentTextColor = percentTextColor;
            HealthTextColor = 0xffffffff;
            HealthTextColorUnder10Percent = 0xffff00ff;
            ShowPercents = showText;
            ShowHealthText = showText;
            TextSize = new RangeNode<int>(15, 10, 50);
        }

        public ColorNode Under10Percent { get; set; }

        public ColorNode PercentTextColor { get; set; }

        public ColorNode HealthTextColor { get; set; }

        public ColorNode HealthTextColorUnder10Percent { get; set; }

        public ToggleNode ShowPercents { get; set; }

        public ToggleNode ShowHealthText { get; set; }

        public RangeNode<int> TextSize { get; set; }
    }
}