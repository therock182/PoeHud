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
            DefaultTextColor = CorruptedAreaColor = Color.White;
        }

        public RangeNode<int> TextSize { get; set; }
        public ColorNode BackgroundColor { get; set; }
        public ColorNode DefaultTextColor { get; set; }
        public ColorNode CorruptedAreaColor { get; set; }
    }
}