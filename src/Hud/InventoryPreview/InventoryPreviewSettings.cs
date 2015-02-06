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
            CellSize = new RangeNode<int>(20, 1, 100);
            CellPadding = new RangeNode<int>(2, 0, 10);
            PositionX = new RangeNode<float>(13.0f, 0.0f, 100.0f);
            PositionY = new RangeNode<float>(76.0f, 0.0f, 100.0f);
        }

        public ColorNode CellUsedColor { get; set; }

        public ColorNode CellFreeColor { get; set; }

        public RangeNode<int> CellSize { get; set; }

        public RangeNode<int> CellPadding { get; set; }

        public RangeNode<float> PositionX { get; set; }

        public RangeNode<float> PositionY { get; set; }
    }
}