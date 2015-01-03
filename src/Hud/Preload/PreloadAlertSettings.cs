using PoeHUD.Hud.Settings;

using SharpDX;

namespace PoeHUD.Hud.Preload
{
    public sealed class PreloadAlertSettings : SettingsBase
    {
        public PreloadAlertSettings()
        {
            Enable = true;
            TextSize = new RangeNode<float>(12, 6, 30);
            BackgroundColor = new ColorBGRA(0, 0, 0, 180);
        }

        public RangeNode<float> TextSize { get; set; }

        public Color BackgroundColor { get; set; }
    }
}