using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.KC
{
    public sealed class KillCounterSettings : SettingsBase
    {
        public KillCounterSettings()
        {
            Enable = false;
            ShowDetail = true;
        }
        public ToggleNode ShowDetail { get; set; }
    }
}