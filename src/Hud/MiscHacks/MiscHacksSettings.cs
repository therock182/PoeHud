using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.MiscHacks
{
    public sealed class MiscHacksSettings : SettingsBase
    {
        public MiscHacksSettings()
        {
            Enable = true;
            Maphack = false;
            Zoomhack = false;
            Fullbright = false;
            Particles = false;
        }

        public ToggleNode Maphack { get; set; }

        public ToggleNode Zoomhack { get; set; }

        public ToggleNode Fullbright { get; set; }

        public ToggleNode Particles { get; set; }
    }
}