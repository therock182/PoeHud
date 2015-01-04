using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.Health;
using PoeHUD.Hud.Settings;
using PoeHUD.Hud.UI;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.Menu
{
    public class MenuPlugin : Plugin<MenuSettings>
    {
        private readonly SettingsHub settingsHub;

        private readonly MouseHook hook;

        private RectangleF bounds;

        private List<ToggleButton> buttons;

        private ToggleButton currentHover;

        private bool menuVisible;

        private bool holdKey;

        public MenuPlugin(GameController gameController, Graphics graphics, SettingsHub settingsHub)
            : base(gameController, graphics, settingsHub.MenuSettings)
        {
            this.settingsHub = settingsHub;
            bounds = new RectangleF(Settings.PositionWidth, Settings.PositionHeight, Settings.Length, Settings.Size);
            CreateButtons();
            hook = new MouseHook(OnMouseEvent);
        }

        public override void Dispose()
        {
            hook.Dispose();
        }

        public override void Render(Dictionary<UiMountPoint, Vector2> mountPoints)
        {
            if (!holdKey && Imports.IsKeyDown(Keys.F12))
            {
                holdKey = true;
                Settings.Enable.Value = !Settings.Enable.Value;
            }
            else if (holdKey && !Imports.IsKeyDown(Keys.F12))
            {
                holdKey = false;
            }

            if (!Settings.Enable)
            {
                return;
            }

            Color boxColor = Color.Gray;
            boxColor.A = menuVisible ? (byte)255 : (byte)100;
            Graphics.DrawBox(bounds, boxColor);
            var position = new Vector2(Settings.PositionWidth + 25, Settings.PositionHeight + 12);
            // TODO textSize to Settings
            Graphics.DrawText("Menu", 17, position, Color.Gray, FontDrawFlags.VerticalCenter | FontDrawFlags.Center);
            buttons.ForEach(x => x.Render(Graphics));
        }

        private static ToggleButton AddButton(MenuItem parent, string text, ToggleNode node)
        {
            var booleanButton = new ToggleButton(text, node);
            parent.AddChild(booleanButton);
            return booleanButton;
        }

        private void CreateButtons()
        {
            int r = 0;
            buttons = new List<ToggleButton>();
            HealthBarSettings healthBarPlugin = settingsHub.HealthBarSettings;
            ToggleButton parent = CreateRootMenu("Health bars", r++, healthBarPlugin.Enable);
            ToggleButton toggleButton = AddButton(parent, "Players", healthBarPlugin.Players.Enable);
            ToggleButton parent2 = AddButton(parent, "Enemies", healthBarPlugin.ShowEnemies);
            ToggleButton toggleButton2 = AddButton(parent, "Minions", healthBarPlugin.Minions.Enable);
            AddButton(parent, "Show ES", healthBarPlugin.ShowES);
            AddButton(parent, "Show in town", healthBarPlugin.ShowInTown);
            toggleButton.AddChild(new Picker<float>("Width", healthBarPlugin.Players.Width));
            toggleButton.AddChild(new Picker<float>("Height", healthBarPlugin.Players.Height));
            toggleButton2.AddChild(new Picker<float>("Width", healthBarPlugin.Minions.Width));
            toggleButton2.AddChild(new Picker<float>("Height", healthBarPlugin.Minions.Height));
            ToggleButton toggleButton3 = AddButton(parent2, "White", healthBarPlugin.NormalEnemy.Enable);
            toggleButton3.AddChild(new ToggleButton("Print percents", healthBarPlugin.NormalEnemy.ShowPercents));
            toggleButton3.AddChild(new ToggleButton("Print health text", healthBarPlugin.NormalEnemy.ShowHealthText));
            toggleButton3.AddChild(new Picker<float>("Width", healthBarPlugin.NormalEnemy.Width));
            toggleButton3.AddChild(new Picker<float>("Height", healthBarPlugin.NormalEnemy.Height));
            ToggleButton toggleButton4 = AddButton(parent2, "Magic", healthBarPlugin.MagicEnemy.Enable);
            toggleButton4.AddChild(new ToggleButton("Print percents", healthBarPlugin.MagicEnemy.ShowPercents));
            toggleButton4.AddChild(new ToggleButton("Print health text", healthBarPlugin.MagicEnemy.ShowHealthText));
            toggleButton4.AddChild(new Picker<float>("Width", healthBarPlugin.MagicEnemy.Width));
            toggleButton4.AddChild(new Picker<float>("Height", healthBarPlugin.MagicEnemy.Height));
            ToggleButton toggleButton5 = AddButton(parent2, "Rare", healthBarPlugin.RareEnemy.Enable);
            toggleButton5.AddChild(new ToggleButton("Print percents", healthBarPlugin.RareEnemy.ShowPercents));
            toggleButton5.AddChild(new ToggleButton("Print health text", healthBarPlugin.RareEnemy.ShowHealthText));
            toggleButton5.AddChild(new Picker<float>("Width", healthBarPlugin.RareEnemy.Width));
            toggleButton5.AddChild(new Picker<float>("Height", healthBarPlugin.RareEnemy.Height));
            ToggleButton toggleButton6 = AddButton(parent2, "Uniques", healthBarPlugin.UniqueEnemy.Enable);
            toggleButton6.AddChild(new ToggleButton("Print percents", healthBarPlugin.UniqueEnemy.ShowPercents));
            toggleButton6.AddChild(new ToggleButton("Print health text", healthBarPlugin.UniqueEnemy.ShowHealthText));
            toggleButton6.AddChild(new Picker<float>("Width", healthBarPlugin.UniqueEnemy.Width));
            toggleButton6.AddChild(new Picker<float>("Height", healthBarPlugin.UniqueEnemy.Height));
            ToggleButton mapIconsRoot = CreateRootMenu("Map icons", r++, settingsHub.MapIconsSettings.Enable);
            AddButton(mapIconsRoot, "Icons on minimap", settingsHub.MapIconsSettings.IconsOnMinimap);
            AddButton(mapIconsRoot, "Icons on large map", settingsHub.MapIconsSettings.IconsOnLargeMap);
            AddButton(mapIconsRoot, "Precious items", settingsHub.ItemAlertSettings.ShowItemOnMap);
            AddButton(mapIconsRoot, "Monsters", settingsHub.MonsterTrackerSettings.Monsters);
            AddButton(mapIconsRoot, "Minions", settingsHub.MonsterTrackerSettings.Minions);
            AddButton(mapIconsRoot, "Strongboxes", settingsHub.PoiTrackerSettings.Strongboxes);
            AddButton(mapIconsRoot, "Chests", settingsHub.PoiTrackerSettings.Chests);
            AddButton(mapIconsRoot, "Masters", settingsHub.PoiTrackerSettings.Masters);
            ToggleButton parent4 = CreateRootMenu("Item alert", r++, settingsHub.ItemAlertSettings.Enable);
            AddButton(parent4, "Rares", settingsHub.ItemAlertSettings.Rares);
            AddButton(parent4, "Uniques", settingsHub.ItemAlertSettings.Uniques);
            AddButton(parent4, "Currency", settingsHub.ItemAlertSettings.Currency);
            AddButton(parent4, "Maps", settingsHub.ItemAlertSettings.Maps);
            AddButton(parent4, "RGB", settingsHub.ItemAlertSettings.Rgb);
            AddButton(parent4, "Crafting bases", settingsHub.ItemAlertSettings.Crafting);
            AddButton(parent4, "Skill gems", settingsHub.ItemAlertSettings.SkillGems);
            AddButton(parent4, "Only quality gems", settingsHub.ItemAlertSettings.QualitySkillGems);
            AddButton(parent4, "Play sound", settingsHub.ItemAlertSettings.PlaySound);
            ToggleButton toggleButton7 = AddButton(parent4, "Show text", settingsHub.ItemAlertSettings.ShowText);
            toggleButton7.AddChild(new Picker<int>("Font size", settingsHub.ItemAlertSettings.TextSize));
            ToggleButton showBorderToggleButton = AddButton(parent4, "Show border", settingsHub.ItemAlertSettings.ShowBorder);
            showBorderToggleButton.AddChild(new Picker<int>("Border weight", settingsHub.ItemAlertSettings.BorderWidth));
            CreateRootMenu("Item level", r++, settingsHub.ItemLevelSettings.Enable);
            ToggleButton itemModsRoot = CreateRootMenu("Item mods", r++, settingsHub.ItemRollsSettings.Enable);
            AddButton(itemModsRoot, "Weapon DPS", settingsHub.ItemRollsSettings.ShowWeaponDps);
            ToggleButton parent5 = CreateRootMenu("Boss warnings", r++, settingsHub.MonsterTrackerSettings.Enable);
            AddButton(parent5, "Sound warning", settingsHub.MonsterTrackerSettings.PlaySound);
            ToggleButton toggleButton8 = AddButton(parent5, "Text warning", settingsHub.MonsterTrackerSettings.ShowText);
            toggleButton8.AddChild(new Picker<int>("Font size", settingsHub.MonsterTrackerSettings.TextSize));
            ToggleButton toggleButton9 = CreateRootMenu("Xph Display", r++, settingsHub.XpRateSettings.Enable);
            toggleButton9.AddChild(new Picker<int>("Font size", settingsHub.XpRateSettings.TextSize));
            ToggleButton parent6 = CreateRootMenu("Client hacks", r++, settingsHub.MiscHacksSettings.Enable);
            AddButton(parent6, "Maphack", settingsHub.MiscHacksSettings.Maphack);
            AddButton(parent6, "Zoomhack", settingsHub.MiscHacksSettings.Zoomhack);
            AddButton(parent6, "Fullbright", settingsHub.MiscHacksSettings.Fullbright);
            AddButton(parent6, "Disable Particles", settingsHub.MiscHacksSettings.Particles);
            ToggleButton toggleButton10 = CreateRootMenu("Preload Alert", r++, settingsHub.PreloadAlertSettings.Enable);
            toggleButton10.AddChild(new Picker<int>("Font size", settingsHub.PreloadAlertSettings.TextSize));
            ToggleButton dpsRoot = CreateRootMenu("Show DPS", r++, settingsHub.DpsMeterSettings.Enable);
            dpsRoot.AddChild(new Picker<int>("DPS font size", settingsHub.DpsMeterSettings.DpsTextSize));
            dpsRoot.AddChild(new Picker<int>("Peak DPS font size", settingsHub.DpsMeterSettings.PeakDpsTextSize));
            dpsRoot.AddChild(new ColorButton("Background color:", settingsHub.DpsMeterSettings.BackgroundColor));
        }

        private ToggleButton CreateRootMenu(string text, int yIndex, ToggleNode node)
        {
            var booleanButton = new ToggleButton(text, node);
            booleanButton.Bounds = new RectangleF(Settings.PositionWidth, Settings.PositionHeight + Settings.Size
                + yIndex * booleanButton.DesiredHeight, booleanButton.DesiredWidth, booleanButton.DesiredHeight);
            buttons.Add(booleanButton);
            return booleanButton;
        }

        private bool OnMouseEvent(MouseEventID id, int x, int y)
        {
            if (!Settings.Enable || !GameController.Window.IsForeground())
            {
                return false;
            }

            Vector2 mousePosition = GameController.Window.ScreenToClient(x, y);
            if (currentHover != null && currentHover.TestHit(mousePosition))
            {
                currentHover.OnEvent(id, mousePosition);
                return id != MouseEventID.MouseMove;
            }
            if (id == MouseEventID.MouseMove)
            {
                ToggleButton button = buttons.FirstOrDefault(b => b.TestHit(mousePosition));
                if (button != null)
                {
                    if (currentHover != null)
                    {
                        currentHover.SetHovered(false);
                    }
                    currentHover = button;
                    button.SetHovered(true);
                }
                return false;
            }
            if (bounds.Contains(mousePosition) && id == MouseEventID.LeftButtonDown)
            {
                menuVisible = !menuVisible;
                buttons.ForEach(button => button.SetVisible(menuVisible));
                return true;
            }
            return false;
        }
    }
}