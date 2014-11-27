using System.Collections.Generic;
using System.Drawing;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.Menu
{
    public class Menu : HudPluginBase
    {
        private const int ButtonWidth = 210;
        private const int ButtonHeight = 40;
        private readonly MouseHook hook;
        private Rect bounds;
        private List<BooleanButton> buttons;
        private BooleanButton currentHover;
        private bool menuVisible;


        public Menu(GameController gameController) : base(gameController)
        {
            bounds = new Rect(Settings.GetInt("Menu.PositionWidth"), Settings.GetInt("Menu.PositionHeight"),
                Settings.GetInt("Menu.Length"), Settings.GetInt("Menu.Size"));
            CreateButtons();
            hook = new MouseHook(OnMouseEvent);
        }

        public override void Dispose()
        {
            hook.Dispose();
        }

        public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
        {
            int alpha = menuVisible ? 255 : 100;
            rc.AddBox(bounds, Color.FromArgb(alpha, Color.Gray));
            rc.AddTextWithHeight(
                new Vec2(Settings.GetInt("Menu.PositionWidth") + 25, Settings.GetInt("Menu.PositionHeight") + 12),
                "Menu", Color.Gray, 10, DrawTextFormat.VerticalCenter | DrawTextFormat.Center);
            foreach (BooleanButton current in buttons)
            {
                current.Render(rc);
            }
        }


        private bool OnMouseEvent(MouseEventID id, int x, int y)
        {
            if (Settings.GetBool("Window.RequireForeground") && !GameController.Window.IsForeground())
            {
                return false;
            }
            Vec2 vec = GameController.Window.ScreenToClient(new Vec2(x, y));
            if (id == MouseEventID.MouseMove)
            {
                if (currentHover != null && currentHover.TestHit(vec))
                {
                    currentHover.OnEvent(id, vec);
                    return false;
                }
                using (List<BooleanButton>.Enumerator enumerator = buttons.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        BooleanButton current = enumerator.Current;
                        if (current.TestHit(vec))
                        {
                            if (currentHover != null)
                            {
                                currentHover.SetHovered(false);
                            }
                            currentHover = current;
                            current.SetHovered(true);
                            return false;
                        }
                    }
                    return false;
                }
            }
            if (bounds.HasPoint(vec) && id == MouseEventID.LeftButtonDown)
            {
                menuVisible = !menuVisible;
                foreach (BooleanButton current2 in buttons)
                {
                    current2.SetVisible(menuVisible);
                }
                return true;
            }
            if (currentHover != null && currentHover.TestHit(vec))
            {
                currentHover.OnEvent(id, vec);
                return true;
            }
            return false;
        }

        private void CreateButtons()
        {
            int r = 0;
            buttons = new List<BooleanButton>();
            BooleanButton parent = CreateRootMenu("Health bars", r++, "Healthbars");
            BooleanButton booleanButton = AddButton(parent, "Players", "Healthbars.Players");
            BooleanButton parent2 = AddButton(parent, "Enemies", "Healthbars.Enemies");
            BooleanButton booleanButton2 = AddButton(parent, "Minions", "Healthbars.Minions");
            AddButton(parent, "Show ES", "Healthbars.ShowES");
            AddButton(parent, "Show in town", "Healthbars.ShowInTown");
            booleanButton.AddChild(new IntPicker("Width", 50, 180, "Healthbars.Players.Width"));
            booleanButton.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Players.Height"));
            booleanButton2.AddChild(new IntPicker("Width", 50, 180, "Healthbars.Minions.Width"));
            booleanButton2.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Minions.Height"));
            BooleanButton booleanButton3 = AddButton(parent2, "White", "Healthbars.Enemies.Normal");
            booleanButton3.AddChild(new BooleanButton("Print percents", "Healthbars.Enemies.Normal.PrintPercents"));
            booleanButton3.AddChild(new BooleanButton("Print health text", "Healthbars.Enemies.Normal.PrintHealthText"));
            booleanButton3.AddChild(new IntPicker("Width", 50, 180, "Healthbars.Enemies.Normal.Width"));
            booleanButton3.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Enemies.Normal.Height"));
            BooleanButton booleanButton4 = AddButton(parent2, "Magic", "Healthbars.Enemies.Magic");
            booleanButton4.AddChild(new BooleanButton("Print percents", "Healthbars.Enemies.Magic.PrintPercents"));
            booleanButton4.AddChild(new BooleanButton("Print health text", "Healthbars.Enemies.Magic.PrintHealthText"));
            booleanButton4.AddChild(new IntPicker("Width", 50, 180, "Healthbars.Enemies.Magic.Width"));
            booleanButton4.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Enemies.Magic.Height"));
            BooleanButton booleanButton5 = AddButton(parent2, "Rare", "Healthbars.Enemies.Rare");
            booleanButton5.AddChild(new BooleanButton("Print percents", "Healthbars.Enemies.Rare.PrintPercents"));
            booleanButton5.AddChild(new BooleanButton("Print health text", "Healthbars.Enemies.Rare.PrintHealthText"));
            booleanButton5.AddChild(new IntPicker("Width", 50, 180, "Healthbars.Enemies.Rare.Width"));
            booleanButton5.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Enemies.Rare.Height"));
            BooleanButton booleanButton6 = AddButton(parent2, "Uniques", "Healthbars.Enemies.Unique");
            booleanButton6.AddChild(new BooleanButton("Print percents", "Healthbars.Enemies.Unique.PrintPercents"));
            booleanButton6.AddChild(new BooleanButton("Print health text", "Healthbars.Enemies.Unique.PrintHealthText"));
            booleanButton6.AddChild(new IntPicker("Width", 50, 180, "Healthbars.Enemies.Unique.Width"));
            booleanButton6.AddChild(new IntPicker("Height", 10, 50, "Healthbars.Enemies.Unique.Height"));
            BooleanButton parent3 = CreateRootMenu("Minimap icons", r++, "MinimapIcons");
            AddButton(parent3, "Monsters", "MinimapIcons.Monsters");
            AddButton(parent3, "Minions", "MinimapIcons.Minions");
            AddButton(parent3, "Strongboxes", "MinimapIcons.Strongboxes");
            AddButton(parent3, "Chests", "MinimapIcons.Chests");
            AddButton(parent3, "Alert items", "MinimapIcons.AlertedItems");
            AddButton(parent3, "Masters", "MinimapIcons.Masters");
            BooleanButton parent4 = CreateRootMenu("Item alert", r++, "ItemAlert");
            AddButton(parent4, "Rares", "ItemAlert.Rares");
            AddButton(parent4, "Uniques", "ItemAlert.Uniques");
            AddButton(parent4, "Currency", "ItemAlert.Currency");
            AddButton(parent4, "Maps", "ItemAlert.Maps");
            AddButton(parent4, "RGB", "ItemAlert.RGB");
            AddButton(parent4, "Crafting bases", "ItemAlert.Crafting");
            AddButton(parent4, "Skill gems", "ItemAlert.SkillGems");
            AddButton(parent4, "Only quality gems", "ItemAlert.QualitySkillGems");
            AddButton(parent4, "Play sound", "ItemAlert.PlaySound");
            BooleanButton booleanButton7 = AddButton(parent4, "Show text", "ItemAlert.ShowText");
            booleanButton7.AddChild(new IntPicker("Font size", 6, 30, "ItemAlert.ShowText.FontSize"));
            BooleanButton tooltip = CreateRootMenu("Advanced tooltips", r++, "Tooltip");
            AddButton(tooltip, "Item level on hover", "Tooltip.ShowItemLevel");
            AddButton(tooltip, "Item mods on hover", "Tooltip.ShowItemMods");
            BooleanButton parent5 = CreateRootMenu("Boss warnings", r++, "MonsterTracker");
            AddButton(parent5, "Sound warning", "MonsterTracker.PlaySound");
            BooleanButton booleanButton8 = AddButton(parent5, "Text warning", "MonsterTracker.ShowText");
            booleanButton8.AddChild(new IntPicker("Font size", 6, 30, "MonsterTracker.ShowText.FontSize"));
            booleanButton8.AddChild(new IntPicker("Background alpha", 0, 200, "MonsterTracker.ShowText.BgAlpha"));
            BooleanButton booleanButton9 = CreateRootMenu("Xph Display", r++, "XphDisplay");
            booleanButton9.AddChild(new IntPicker("Font size", 6, 30, "XphDisplay.FontSize"));
            booleanButton9.AddChild(new IntPicker("Background alpha", 0, 200, "XphDisplay.BgAlpha"));
            BooleanButton parent6 = CreateRootMenu("Client hacks", r++, "ClientHacks");
            AddButton(parent6, "Maphack", "ClientHacks.Maphack");
            AddButton(parent6, "Zoomhack", "ClientHacks.Zoomhack");
            AddButton(parent6, "Fullbright", "ClientHacks.Fullbright");
            AddButton(parent6, "Disable Particles", "ClientHacks.Particles");
            BooleanButton booleanButton10 = CreateRootMenu("Preload Alert", r++, "PreloadAlert");
            booleanButton10.AddChild(new IntPicker("Font size", 6, 30, "PreloadAlert.FontSize"));
            booleanButton10.AddChild(new IntPicker("Background alpha", 0, 200, "PreloadAlert.BgAlpha"));
            BooleanButton dps = CreateRootMenu("Show DPS", r++, "DpsDisplay");
            // BooleanButton closeWithGame = this.CreateRootMenu("Exit when Game is closed", 8, "ExitWithGame");
        }

        private BooleanButton AddButton(BooleanButton parent, string text, string setting)
        {
            var booleanButton = new BooleanButton(text, setting);
            parent.AddChild(booleanButton);
            return booleanButton;
        }

        private BooleanButton CreateRootMenu(string text, int yIndex, string setting)
        {
            var booleanButton = new BooleanButton(text, setting);
            booleanButton.Bounds = new Rect(Settings.GetInt("Menu.PositionWidth"),
                Settings.GetInt("Menu.PositionHeight") + Settings.GetInt("Menu.Size") +
                yIndex*booleanButton.DesiredHeight, booleanButton.DesiredWidth, booleanButton.DesiredHeight);
            buttons.Add(booleanButton);
            return booleanButton;
        }
    }
}