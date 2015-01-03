using PoeHUD.Hud.Settings;

using SharpDX;

namespace PoeHUD.Hud.Monster
{
    public sealed class MonsterTrackerSettings : SettingsBase
    {
        public MonsterTrackerSettings()
        {
            Enable = true;
            PlaySound = true;
            ShowText = true;
            TextSize = new RangeNode<float>(16, 6, 30);
            BackgroundColor = new ColorBGRA(0, 0, 0, 128);
        }

        public ToggleNode PlaySound { get; set; }

        public ToggleNode ShowText { get; set; }

        public RangeNode<float> TextSize { get; set; }

        public Color BackgroundColor { get; set; }
    }
}