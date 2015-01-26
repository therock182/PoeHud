using PoeHUD.Controllers;
using PoeHUD.Framework.Helpers;
using PoeHUD.Hud.UI;
using PoeHUD.Poe.RemoteMemoryObjects;
using PoeHUD.Poe.UI;
using SharpDX;
using System;

namespace PoeHUD.Hud.InventoryPreview
{
    public class InventoryPreviewPlugin : Plugin<InventoryPreviewSettings>
    {
        private const int CELLS_Y_COUNT = 5;

        private const int CELLS_X_COUNT = 12;

        private CellData[,] cells;

        public InventoryPreviewPlugin(GameController gameController, Graphics graphics, InventoryPreviewSettings settings)
            : base(gameController, graphics, settings) {}

        public override void Render()
        {
            if (!Settings.Enable || GameController.Game.IngameState.IngameUi.OpenLeftPanel.IsVisible)
            {
                return;
            }

            cells = new CellData[CELLS_Y_COUNT, CELLS_X_COUNT];
            for (int y = 0; y < CELLS_Y_COUNT; ++y) {
                for (int x = 0; x < CELLS_X_COUNT; ++x) {
                    cells[y, x] = new CellData();
                }
            }
            AddItems();

            RectangleF rect = GameController.Window.GetWindowRectangle();
            float xPos = rect.Width * Settings.PositionX / 100 + rect.X;
            float yPos = rect.Height * Settings.PositionY / 100 + rect.Y;
            var startDrawPoint = new Vector2(xPos, yPos);
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    Vector2 d = startDrawPoint.Translate(j * Settings.CellSize, i * Settings.CellSize);
                    float cellWidth = cells[i, j].ExtendsX ? Settings.CellSize : Math.Max(1, Settings.CellSize - Settings.CellPadding);
                    float cellHeight = cells[i, j].ExtendsY ? Settings.CellSize : Math.Max(1, Settings.CellSize - Settings.CellPadding);
                    var rectangleF = new RectangleF(d.X, d.Y, cellWidth, cellHeight);
                    Graphics.DrawBox(rectangleF, cells[i, j].Used ? Settings.CellUsedColor : Settings.CellFreeColor);
                }
            }
        }

        private void AddItem(int x, int y, Size2 itemSize)
        {
            for (int i = y; i < itemSize.Height + y; i++)
            {
                for (int j = x; j < itemSize.Width + x; j++)
                {
                    cells[i, j].Used = true;
                    if (j < itemSize.Width + x - 1) {
                        cells[i, j].ExtendsX = true;
                    }
                    if (i < itemSize.Height + y - 1) {
                        cells[i, j].ExtendsY = true;
                    }
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
                AddItem(x, y, itemSize);
            }
        }
    }
}