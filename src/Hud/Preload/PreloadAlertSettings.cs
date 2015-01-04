using PoeHUD.Hud.Settings;

using SharpDX;

namespace PoeHUD.Hud.Preload
{
    public sealed class PreloadAlertSettings : SettingsBase
    {
        public PreloadAlertSettings()
        {
            Enable = true;
            TextSize = new RangeNode<int>(20, 10, 50);
            BackgroundColor = new ColorBGRA(0, 0, 0, 180);
        }

        public RangeNode<int> TextSize { get; set; }

        public ColorNode BackgroundColor { get; set; }
    }
}