using PoeHUD.Hud.Settings;

using SharpDX;

namespace PoeHUD.Hud.InventoryPreview
{
    public sealed class InventoryPreviewSettings : SettingsBase
    {
        public InventoryPreviewSettings()
        {
            Enable = false;
            CellUsedColor = new Color(255, 0, 0, 80);
            CellFreeColor = new Color(0, 255, 0, 80);
        }

        public ColorNode CellUsedColor { get; set; }

        public ColorNode CellFreeColor { get; set; }
    }
}