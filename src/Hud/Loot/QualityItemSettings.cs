using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.Loot
{
    public sealed class QualityItemSettings : SettingsBase
    {
        public QualityItemSettings() {}

        public QualityItemSettings(int minQuality)
        {
            Enable = true;
            MinQuality = new RangeNode<int>(minQuality, 0, 20);
        }

        public RangeNode<int> MinQuality { get; set; }
    }
}