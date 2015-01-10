using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.AdvancedTooltip
{
    public sealed class WeaponDpsSettings : SettingsBase
    {
        public WeaponDpsSettings()
        {
            Enable = true;
            DpsTextSize = new RangeNode<int>(20, 10, 50);
            DpsNameTextSize = new RangeNode<int>(13, 10, 50);
        }

        public RangeNode<int> DpsTextSize { get; set; }

        public RangeNode<int> DpsNameTextSize { get; set; }
    }
}