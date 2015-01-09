using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.Dps
{
    public sealed class WeaponDpsSettings : SettingsBase
    {
        public WeaponDpsSettings()
        {
            Enable = true;
            DpsTextSize = new RangeNode<int>(20, 10, 50);
            DpsNameTextSize = new RangeNode<int>(13, 10, 50);
            OffsetInnerX = new RangeNode<int>(2, 0, 10);
            OffsetInnerY = new RangeNode<int>(1, 0, 10);
        }

        public RangeNode<int> DpsTextSize { get; set; }

        public RangeNode<int> DpsNameTextSize { get; set; }

        public RangeNode<int> OffsetInnerX { get; set; }

        public RangeNode<int> OffsetInnerY { get; set; }
    }
}