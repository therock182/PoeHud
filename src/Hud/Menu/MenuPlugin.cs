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

            // Health bars
            HealthBarSettings healthBarPlugin = settingsHub.HealthBarSettings;
            ToggleButton healthMenu = CreateRootMenu("Health bars", r++, healthBarPlugin.Enable);
            ToggleButton playersMenu = AddButton(healthMenu, "Players", healthBarPlugin.Players.Enable);
            ToggleButton enemiesMenu = AddButton(healthMenu, "Enemies", healthBarPlugin.ShowEnemies);
            ToggleButton minionsMenu = AddButton(healthMenu, "Minions", healthBarPlugin.Minions.Enable);
            AddButton(healthMenu, "Show ES", healthBarPlugin.ShowES);
            AddButton(healthMenu, "Show in town", healthBarPlugin.ShowInTown);
            playersMenu.AddChild(new Picker<float>("Width", healthBarPlugin.Players.Width));
            playersMenu.AddChild(new Picker<float>("Height", healthBarPlugin.Players.Height));
            minionsMenu.AddChild(new Picker<float>("Width", healthBarPlugin.Minions.Width));
            minionsMenu.AddChild(new Picker<float>("Height", healthBarPlugin.Minions.Height));
            ToggleButton whiteEnemyMenu = AddButton(enemiesMenu, "White", healthBarPlugin.NormalEnemy.Enable);
            whiteEnemyMenu.AddChild(new ToggleButton("Print percents", healthBarPlugin.NormalEnemy.ShowPercents));
            whiteEnemyMenu.AddChild(new ToggleButton("Print health text", healthBarPlugin.NormalEnemy.ShowHealthText));
            whiteEnemyMenu.AddChild(new Picker<float>("Width", healthBarPlugin.NormalEnemy.Width));
            whiteEnemyMenu.AddChild(new Picker<float>("Height", healthBarPlugin.NormalEnemy.Height));
            ToggleButton magicEnemyMenu = AddButton(enemiesMenu, "Magic", healthBarPlugin.MagicEnemy.Enable);
            magicEnemyMenu.AddChild(new ToggleButton("Print percents", healthBarPlugin.MagicEnemy.ShowPercents));
            magicEnemyMenu.AddChild(new ToggleButton("Print health text", healthBarPlugin.MagicEnemy.ShowHealthText));
            magicEnemyMenu.AddChild(new Picker<float>("Width", healthBarPlugin.MagicEnemy.Width));
            magicEnemyMenu.AddChild(new Picker<float>("Height", healthBarPlugin.MagicEnemy.Height));
            ToggleButton rareEnemyMenu = AddButton(enemiesMenu, "Rare", healthBarPlugin.RareEnemy.Enable);
            rareEnemyMenu.AddChild(new ToggleButton("Print percents", healthBarPlugin.RareEnemy.ShowPercents));
            rareEnemyMenu.AddChild(new ToggleButton("Print health text", healthBarPlugin.RareEnemy.ShowHealthText));
            rareEnemyMenu.AddChild(new Picker<float>("Width", healthBarPlugin.RareEnemy.Width));
            rareEnemyMenu.AddChild(new Picker<float>("Height", healthBarPlugin.RareEnemy.Height));
            ToggleButton uniquesEnemyMenu = AddButton(enemiesMenu, "Uniques", healthBarPlugin.UniqueEnemy.Enable);
            uniquesEnemyMenu.AddChild(new ToggleButton("Print percents", healthBarPlugin.UniqueEnemy.ShowPercents));
            uniquesEnemyMenu.AddChild(new ToggleButton("Print health text", healthBarPlugin.UniqueEnemy.ShowHealthText));
            uniquesEnemyMenu.AddChild(new Picker<float>("Width", healthBarPlugin.UniqueEnemy.Width));
            uniquesEnemyMenu.AddChild(new Picker<float>("Height", healthBarPlugin.UniqueEnemy.Height));

            // Map icons
            ToggleButton mapIconsMenu = CreateRootMenu("Map icons", r++, settingsHub.MapIconsSettings.Enable);
            AddButton(mapIconsMenu, "Icons on minimap", settingsHub.MapIconsSettings.IconsOnMinimap);
            AddButton(mapIconsMenu, "Icons on large map", settingsHub.MapIconsSettings.IconsOnLargeMap);
            AddButton(mapIconsMenu, "Precious items", settingsHub.ItemAlertSettings.ShowItemOnMap);
            AddButton(mapIconsMenu, "Monsters", settingsHub.MonsterTrackerSettings.Monsters);
            AddButton(mapIconsMenu, "Minions", settingsHub.MonsterTrackerSettings.Minions);
            AddButton(mapIconsMenu, "Strongboxes", settingsHub.PoiTrackerSettings.Strongboxes);
            AddButton(mapIconsMenu, "Chests", settingsHub.PoiTrackerSettings.Chests);
            AddButton(mapIconsMenu, "Masters", settingsHub.PoiTrackerSettings.Masters);

            // Item Alert
            ToggleButton itemAlertMenu = CreateRootMenu("Item alert", r++, settingsHub.ItemAlertSettings.Enable);
            AddButton(itemAlertMenu, "Rares", settingsHub.ItemAlertSettings.Rares);
            AddButton(itemAlertMenu, "Uniques", settingsHub.ItemAlertSettings.Uniques);
            AddButton(itemAlertMenu, "Currency", settingsHub.ItemAlertSettings.Currency);
            AddButton(itemAlertMenu, "Maps", settingsHub.ItemAlertSettings.Maps);
            AddButton(itemAlertMenu, "RGB", settingsHub.ItemAlertSettings.Rgb);
            AddButton(itemAlertMenu, "Crafting bases", settingsHub.ItemAlertSettings.Crafting);
            AddButton(itemAlertMenu, "Skill gems", settingsHub.ItemAlertSettings.SkillGems);
            AddButton(itemAlertMenu, "Only quality gems", settingsHub.ItemAlertSettings.QualitySkillGems);
            AddButton(itemAlertMenu, "Play sound", settingsHub.ItemAlertSettings.PlaySound);
            ToggleButton alertTextMenu = AddButton(itemAlertMenu, "Show text", settingsHub.ItemAlertSettings.ShowText);
            alertTextMenu.AddChild(new Picker<int>("Font size", settingsHub.ItemAlertSettings.TextSize));
            ToggleButton showBorderMenu = AddButton(itemAlertMenu, "Show border", settingsHub.ItemAlertSettings.ShowBorder);
            showBorderMenu.AddChild(new Picker<int>("Border weight", settingsHub.ItemAlertSettings.BorderWidth));
            showBorderMenu.AddChild(new ColorButton("Border color:", settingsHub.ItemAlertSettings.BorderColor));

            // Item level
            ToggleButton itemLevelMenu = CreateRootMenu("Item level", r++, settingsHub.ItemLevelSettings.Enable);
            itemLevelMenu.AddChild(new Picker<int>("Font size", settingsHub.ItemLevelSettings.TextSize));

            // Item mods
            ToggleButton itemModsMenu = CreateRootMenu("Item mods", r++, settingsHub.ItemRollsSettings.Enable);
            itemModsMenu.AddChild(new Picker<int>("Mods size", settingsHub.ItemRollsSettings.ModTextSize));
            ToggleButton weaponDpsMenu = AddButton(itemModsMenu, "Weapon DPS", settingsHub.ItemRollsSettings.ShowWeaponDps);
            weaponDpsMenu.AddChild(new Picker<int>("DPS size", settingsHub.ItemRollsSettings.DpsTextSize));
            weaponDpsMenu.AddChild(new Picker<int>("DPS name size", settingsHub.ItemRollsSettings.DpsNameTextSize));

            // Boss warnings
            ToggleButton bossWarningsMenu = CreateRootMenu("Boss warnings", r++, settingsHub.MonsterTrackerSettings.Enable);
            AddButton(bossWarningsMenu, "Sound warning", settingsHub.MonsterTrackerSettings.PlaySound);
            ToggleButton warningTextMenu = AddButton(bossWarningsMenu, "Text warning", settingsHub.MonsterTrackerSettings.ShowText);
            warningTextMenu.AddChild(new Picker<int>("Font size", settingsHub.MonsterTrackerSettings.TextSize));
            warningTextMenu.AddChild(new ColorButton("Background color:", settingsHub.MonsterTrackerSettings.BackgroundColor));

            // Xph Display
            ToggleButton xpRateMenu = CreateRootMenu("Xph Display", r++, settingsHub.XpRateSettings.Enable);
            xpRateMenu.AddChild(new Picker<int>("Font size", settingsHub.XpRateSettings.TextSize));
            xpRateMenu.AddChild(new ColorButton("Background color:", settingsHub.XpRateSettings.BackgroundColor));

            // Client hacks
            ToggleButton clientHacksMenu = CreateRootMenu("Client hacks", r++, settingsHub.MiscHacksSettings.Enable);
            AddButton(clientHacksMenu, "Maphack", settingsHub.MiscHacksSettings.Maphack);
            AddButton(clientHacksMenu, "Zoomhack", settingsHub.MiscHacksSettings.Zoomhack);
            AddButton(clientHacksMenu, "Fullbright", settingsHub.MiscHacksSettings.Fullbright);
            AddButton(clientHacksMenu, "Disable Particles", settingsHub.MiscHacksSettings.Particles);

            // Preload Alert
            ToggleButton preloadMenu = CreateRootMenu("Preload Alert", r++, settingsHub.PreloadAlertSettings.Enable);
            preloadMenu.AddChild(new Picker<int>("Font size", settingsHub.PreloadAlertSettings.TextSize));
            preloadMenu.AddChild(new ColorButton("Background color:", settingsHub.PreloadAlertSettings.BackgroundColor));

            // Show DPS
            ToggleButton showDpsMenu = CreateRootMenu("Show DPS", r++, settingsHub.DpsMeterSettings.Enable);
            showDpsMenu.AddChild(new Picker<int>("DPS font size", settingsHub.DpsMeterSettings.DpsTextSize));
            showDpsMenu.AddChild(new Picker<int>("Peak DPS font size", settingsHub.DpsMeterSettings.PeakDpsTextSize));
            showDpsMenu.AddChild(new ColorButton("Background color:", settingsHub.DpsMeterSettings.BackgroundColor));
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