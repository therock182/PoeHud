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
            TextSize = new RangeNode<int>(27, 10, 50);
            BackgroundColor = new ColorBGRA(0, 0, 0, 128);
        }

        public ToggleNode PlaySound { get; set; }

        public ToggleNode ShowText { get; set; }

        public RangeNode<int> TextSize { get; set; }

        public Color BackgroundColor { get; set; }
    }
}