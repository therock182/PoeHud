using PoeHUD.Hud.Settings;

using SharpDX;

namespace PoeHUD.Hud.Loot
{
    public sealed class BorderSettings : SettingsBase
    {
        public BorderSettings()
        {
            Enable = true;
            BorderColor = Color.FromAbgr(0xbb252ff);
            CantPickUpBorderColor = Color.Red;
            ShowTimer = true;
            BorderWidth = new RangeNode<int>(3, 1, 10);
            TimerTextSize = new RangeNode<int>(10, 8, 40);
        }

        public ColorNode BorderColor { get; set; }

        public ColorNode CantPickUpBorderColor { get; set; }

        public ToggleNode ShowTimer { get; set; }

        public RangeNode<int> BorderWidth { get; set; }

        public RangeNode<int> TimerTextSize { get; set; }
    }
}