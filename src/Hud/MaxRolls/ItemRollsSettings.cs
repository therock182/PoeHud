using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.MaxRolls
{
    public sealed class ItemRollsSettings : SettingsBase
    {
        public ItemRollsSettings()
        {
            Enable = true;
            ModTextSize = new RangeNode<int>(13, 10, 50);
        }

        public RangeNode<int> ModTextSize { get; set; }
    }
}