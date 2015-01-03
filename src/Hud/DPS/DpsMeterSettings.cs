using PoeHUD.Hud.Settings;

using SharpDX;

namespace PoeHUD.Hud.DPS
{
    public sealed class DpsMeterSettings : SettingsBase
    {
        public DpsMeterSettings()
        {
            Enable = true;
            DpsTextSize = new RangeNode<float>(18, 6, 30);
            PeakDpsTextSize = new RangeNode<float>(8, 6, 30);
            BackgroundColor = new ColorBGRA(0, 0, 0, 160);
        }

        public RangeNode<float> DpsTextSize { get; set; }

        public RangeNode<float> PeakDpsTextSize { get; set; }

        public Color BackgroundColor { get; set; }
    }
}