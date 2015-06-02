using System;

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

        private CellData[,] cells;

        private IngameUIElements ingameUiElements;

        public InventoryPreviewPlugin(GameController gameController, Graphics graphics,
            InventoryPreviewSettings settings)
            : base(gameController, graphics, settings)
        {

            cells = new CellData[CELLS_Y_COUNT, CELLS_X_COUNT];
        }

        public override void Render()
        {
            if (!Settings.Enable)
            {
                return;
            }

            ingameUiElements = GameController.Game.IngameState.IngameUi;
            if (ingameUiElements.OpenLeftPanel.IsVisible || ingameUiElements.OpenRightPanel.IsVisible)
            {
                if (ingameUiElements.InventoryPanel.IsVisible)
                {
                    cells = new CellData[CELLS_Y_COUNT, CELLS_X_COUNT];
                    AddItems();
                }
                return;
            }
            RectangleF rect = GameController.Window.GetWindowRectangle();
            float xPos = rect.Width * Settings.PositionX * .01f;
            float yPos = rect.Height * Settings.PositionY * .01f;
            var startDrawPoint = new Vector2(xPos, yPos);
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    Vector2 d = startDrawPoint.Translate(j * Settings.CellSize, i * Settings.CellSize);
                    float cellWidth = GetCellSize(cells[i, j].ExtendsX);
                    float cellHeight = GetCellSize(cells[i, j].ExtendsY);
                    var rectangleF = new RectangleF(d.X, d.Y, cellWidth, cellHeight);
                    Graphics.DrawBox(rectangleF, cells[i, j].Used ? Settings.CellUsedColor : Settings.CellFreeColor);
                }
            }
        }

        private void AddItem(int x, int y, int maxX, int maxY)
        {
            for (int i = y; i < maxY; i++)
            {
                for (int j = x; j < maxX; j++)
                {
                    cells[i, j].Used = true;
                    cells[i, j].ExtendsX = j < maxX - 1;
                    cells[i, j].ExtendsY = i < maxY - 1;
                }
            }
        }

        private void AddItems()
        {
            var inventoryZone = ingameUiElements.ReadObject<Element>(ingameUiElements.InventoryPanel.Address + 0x808 + 0x24C);
            RectangleF inventoryZoneRectangle = inventoryZone.GetClientRect();
            var oneCellSize = new Size2F(inventoryZoneRectangle.Width / CELLS_X_COUNT,
                inventoryZoneRectangle.Height / CELLS_Y_COUNT);
            foreach (Element itemElement in inventoryZone.Children)
            {
                RectangleF itemElementRectangle = itemElement.GetClientRect();
                var x = (int)((itemElementRectangle.X - inventoryZoneRectangle.X) / oneCellSize.Width + 0.5);
                var y = (int)((itemElementRectangle.Y - inventoryZoneRectangle.Y) / oneCellSize.Height + 0.5);
                int maxX = (int)(itemElementRectangle.Width / oneCellSize.Width + 0.5) + x;
                int maxY = (int)(itemElementRectangle.Height / oneCellSize.Height + 0.5) + y;

                // BUG Old inventoryZoneRectangle when the window is moved/resized
                if (x < 0 || maxX > CELLS_X_COUNT || y < 0 || maxY > CELLS_Y_COUNT)
                {
                    break;
                }

                AddItem(x, y, maxX, maxY);
            }
        }

        private int GetCellSize(bool extendsSide)
        {
            return extendsSide ? Settings.CellSize : Math.Max(1, Settings.CellSize - Settings.CellPadding);
        }
    }
}