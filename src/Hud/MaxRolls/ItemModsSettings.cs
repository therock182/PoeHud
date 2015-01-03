using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.MaxRolls
{
    public sealed class ItemModsSettings : SettingsBase
    {
        public ItemModsSettings()
        {
            Enable = true;
            ShowWeaponDps = true;
        }

        public ToggleNode ShowWeaponDps { get; set; }
    }
}