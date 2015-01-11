using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.Menu
{
    public sealed class MenuSettings : SettingsBase
    {
        public MenuSettings()
        {
            Enable = true;
            X = 10;
            Y = 100;
        }

        public float X { get; set; }

        public float Y { get; set; }
    }
}