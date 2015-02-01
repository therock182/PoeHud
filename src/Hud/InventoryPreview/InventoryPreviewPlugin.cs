using PoeHUD.Controllers;
using PoeHUD.Framework.Helpers;
using PoeHUD.Hud.UI;
using PoeHUD.Poe.RemoteMemoryObjects;
using PoeHUD.Poe.UI;

using SharpDX;

namespace PoeHUD.Hud.InventoryPreview
{
    public class InventoryPreviewPlugin : Plugin<InventoryPreviewSettings>
    {
        private const int CELLS_Y_COUNT = 5;

        private const int CELLS_X_COUNT = 12;

        private bool[,] cells;

        public InventoryPreviewPlugin(GameController gameController, Graphics graphics, InventoryPreviewSettings settings)
            : base(gameController, graphics, settings) {}

        public override void Render()
        {
            if (!Settings.Enable || GameController.Game.IngameState.IngameUi.OpenLeftPanel.IsVisible)
            {
                return;
            }

            cells = new bool[CELLS_Y_COUNT, CELLS_X_COUNT];
            AddItems();

            Element hpGlobe = GameController.Game.IngameState.IngameUi.HpGlobe;
            RectangleF hpGlobeRectangle = hpGlobe.GetClientRect();
            var startDrawPoint = new Vector2(hpGlobeRectangle.X + hpGlobeRectangle.Width, hpGlobe.Children[0].GetClientRect().Y);
            var size = (int)(hpGlobeRectangle.Height * 0.1); //size=20, hbglobe=200.5 -> 20/200.5~0.1
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    Vector2 d = startDrawPoint.Translate(j * size, i * size);
                    var rectangleF = new RectangleF(d.X, d.Y, size - 2f, size - 2f);
                    Graphics.DrawBox(rectangleF, cells[i, j] ? Settings.CellUsedColor : Settings.CellFreeColor);
                }
            }
        }

        private void AddItem(int x, int y, Size2 itemSize)
        {
            for (int i = y; i < itemSize.Height + y; i++)
            {
                for (int j = x; j < itemSize.Width + x; j++)
                {
                    cells[i, j] = true;
                }
            }
        }

        private void AddItems()
        {
            IngameUIElements ui = GameController.Game.IngameState.IngameUi;
            var inventoryZone = ui.ReadObject<Element>(ui.InventoryPanel.Address + 0x808 + 0x248);
            RectangleF inventoryZoneRectangle = inventoryZone.GetClientRect();
            var oneCellSize = new Size2F(inventoryZoneRectangle.Width / CELLS_X_COUNT,
                inventoryZoneRectangle.Height / CELLS_Y_COUNT);
            foreach (Element itemElement in inventoryZone.Children)
            {
                RectangleF itemElementRectangle = itemElement.GetClientRect();
                var x = (int)((itemElementRectangle.X - inventoryZoneRectangle.X) / oneCellSize.Width + 0.5);
                var y = (int)((itemElementRectangle.Y - inventoryZoneRectangle.Y) / oneCellSize.Height + 0.5);
                var itemSize = new Size2((int)(itemElementRectangle.Width / oneCellSize.Width + 0.5),
                    (int)(itemElementRectangle.Height / oneCellSize.Height + 0.5));

                if (x >= 0 && y >= 0) // Prevent index out of bounds (when selling items)
                {
                    AddItem(x, y, itemSize);
                }
            }
        }
    }
}