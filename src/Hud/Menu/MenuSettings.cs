using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.Menu
{
    public sealed class MenuSettings : SettingsBase
    {
        public MenuSettings()
        {
            Enable = true;
            Size = 25;
            Length = 50;
            PositionWidth = 0;
            PositionHeight = 100;
        }

        public float Size { get; set; }

        public float Length { get; set; }

        public float PositionWidth { get; set; }

        public float PositionHeight { get; set; }
    }
}