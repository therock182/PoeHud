using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.DPS;
using PoeHUD.Hud.Health;
using PoeHUD.Hud.Icons;
using PoeHUD.Hud.Interfaces;
using PoeHUD.Hud.Loot;
using PoeHUD.Hud.MaxRolls;
using PoeHUD.Hud.Monster;
using PoeHUD.Hud.Preload;
using PoeHUD.Hud.XpRate;
using PoeHUD.Poe.UI;

using SharpDX;
using SharpDX.Windows;

using Color = System.Drawing.Color;
using Graphics2D = PoeHUD.Hud.UI.Graphics;
using Rectangle = System.Drawing.Rectangle;

namespace PoeHUD.Hud
{
    internal class ExternalOverlay : RenderForm
    {
        private readonly GameController gameController;

        private readonly Func<bool> gameEnded;

        private readonly IntPtr gameHandle;

        private readonly List<Plugin> plugins = new List<Plugin>();

        private Graphics2D graphics;

        public ExternalOverlay(GameController gameController, Func<bool> gameEnded)
        {
            this.gameController = gameController;
            this.gameEnded = gameEnded;
            gameHandle = gameController.Window.Process.MainWindowHandle;

            SuspendLayout();
            string title = Settings.GetString("Window.Name");
            Text = string.IsNullOrWhiteSpace(title) ? "PoeHUD" : title;
            TransparencyKey = Color.Transparent;
            BackColor = Color.Black;
            FormBorderStyle = FormBorderStyle.None;
            ShowIcon = false;
            //ShowInTaskbar = false;
            TopMost = true;
            ResumeLayout(false);
            Load += OnLoad;
        }

        private async void CheckGameState()
        {
            while (!gameEnded())
            {
                await Task.Delay(500);
            }
            graphics.Dispose();
            Close();
        }

        private async void CheckGameWindow()
        {
            while (!gameEnded())
            {
                await Task.Delay(1000);
                Rectangle gameSize = WinApi.GetClientRectangle(gameHandle);
                Bounds = gameSize;
            }
        }

        private IEnumerable<MapIcon> GatherMapIcons()
        {
            IEnumerable<IHudPluginWithMapIcons> pluginsWithIcons = plugins.OfType<IHudPluginWithMapIcons>();
            return pluginsWithIcons.SelectMany(iconSource => iconSource.GetIcons());
        }

        private Vector2 GetLeftCornerMap()
        {
            RectangleF clientRect = gameController.Game.IngameState.IngameUi.Map.SmallMinimap.GetClientRect();
            return new Vector2(clientRect.X - 10, clientRect.Y + 5);
        }

        private Vector2 GetUnderCornerMap()
        {
            Element gemPanel = gameController.Game.IngameState.IngameUi.GemLvlUpPanel;
            RectangleF gemPanelRect = gemPanel.GetClientRect();
            RectangleF mapRect = gameController.Game.IngameState.IngameUi.Map.SmallMinimap.GetClientRect();
            RectangleF clientRect = gemPanel.IsVisible && gemPanelRect.X + gemPanel.Width < mapRect.X + mapRect.X + 50
                ? gemPanel.GetClientRect()
                : mapRect;
            return new Vector2(mapRect.X + mapRect.Width, clientRect.Y + clientRect.Height + 10);
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            plugins.ForEach(plugin => plugin.Dispose());
            graphics.Dispose();
        }

        private void OnDeactivate(object sender, EventArgs e)
        {
            BringToFront();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            Bounds = WinApi.GetClientRectangle(gameHandle);
            WinApi.EnableTransparent(Handle, Bounds);
            graphics = new Graphics2D(this, Bounds.Width, Bounds.Height);
            graphics.Render += OnRender;

            plugins.Add(new HealthBarRenderer(gameController, graphics));
            plugins.Add(new ItemAlerter(gameController, graphics));
            plugins.Add(new MinimapRenderer(gameController, graphics, GatherMapIcons));
            plugins.Add(new LargeMapRenderer(gameController, graphics, GatherMapIcons));
            plugins.Add(new ItemLevelRenderer(gameController, graphics));
            plugins.Add(new ItemRollsRenderer(gameController, graphics));
            plugins.Add(new MonsterTracker(gameController, graphics));
            plugins.Add(new PoiTracker(gameController, graphics));
            plugins.Add(new XPHRenderer(gameController, graphics));
            plugins.Add(new ClientHacks(gameController, graphics));
            plugins.Add(new PreloadAlert(gameController, graphics));
            plugins.Add(new DpsMeter(gameController, graphics));
            //if (Settings.GetBool("Window.ShowIngameMenu"))
            {
                //#if !DEBUG
                plugins.Add(new Menu.Menu(gameController, graphics));
                //#endif
            }

            Deactivate += OnDeactivate;
            FormClosing += OnClosing;

            CheckGameWindow();
            CheckGameState();
            Task.Run(() => graphics.RenderLoop());
        }

        private void OnRender()
        {
            if (gameController.InGame && (!Settings.GetBool("Window.RequireForeground") || WinApi.IsForegroundWindow(gameHandle)))
            {
                gameController.RefreshState();

                var mountPoints = new Dictionary<UiMountPoint, Vector2>();
                mountPoints[UiMountPoint.UnderMinimap] = GetUnderCornerMap();
                mountPoints[UiMountPoint.LeftOfMinimap] = GetLeftCornerMap();
                plugins.ForEach(x => x.Render(mountPoints));
            }
        }
    }
}