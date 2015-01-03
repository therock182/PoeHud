using PoeHUD.Hud.Settings;

using SharpDX;

namespace PoeHUD.Hud.DPS
{
    public sealed class DpsMeterSettings : SettingsBase
    {
        public DpsMeterSettings()
        {
            Enable = true;
            DpsTextSize = new RangeNode<int>(30, 10, 50);
            PeakDpsTextSize = new RangeNode<int>(13, 10, 50);
            BackgroundColor = new ColorBGRA(0, 0, 0, 160);
        }

        public RangeNode<int> DpsTextSize { get; set; }

        public RangeNode<int> PeakDpsTextSize { get; set; }

        public Color BackgroundColor { get; set; }
    }
}