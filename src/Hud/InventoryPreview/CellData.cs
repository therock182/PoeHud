
namespace PoeHUD.Hud.InventoryPreview
{
    class CellData
    {
        public CellData()
        {
            Used = false;
            ExtendsX = false;
            ExtendsY = false;
        }

        public bool Used { get; set; }

        public bool ExtendsX { get; set; }

        public bool ExtendsY { get; set; }
    }
}