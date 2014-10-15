using System.Collections.Generic;
using System.Drawing;
using PoeHUD.Framework;
using PoeHUD.Poe;
using PoeHUD.Game;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.UI;
using SlimDX.Direct3D9;
using PoeHUD.ExileBot;

namespace PoeHUD.Hud.debugwin
{
    class debugWindowRenderer : HUDPlugin
    {
        private int lines;
        private Rect destWin;

        public override void OnEnable()
        {
        }

        public override void OnDisable()
        {
        }

        private void addLine(RenderingContext rc,string t)
        {
            rc.AddTextWithHeight(new Vec2(destWin.X , destWin.Y + (lines * 16)), t, Color.White, 8, DrawTextFormat.Left);
            lines++;
        }

        public override void Render(RenderingContext rc)
        {
            if (Settings.GetBool("debug"))
            {
                lines = 0;

                var mm = this.poe.Internal.game.IngameState.IngameUi.Minimap.SmallMinimap;
                var qt = this.poe.Internal.game.IngameState.IngameUi.QuestTracker;
                Rect miniMapRect = mm.GetClientRect();
                Rect qtRect = qt.GetClientRect();

                Rect clientRect;
                if (qt.IsVisible && qtRect.X + qt.Width < miniMapRect.X + miniMapRect.X + 50)
                    clientRect = qtRect;
                else
                    clientRect = miniMapRect;
                int xOffs = 3;
                destWin = new Rect(clientRect.X, clientRect.Y + clientRect.H + 5, clientRect.W, clientRect.H);
                rc.AddBox(destWin, Color.FromArgb(180, 0, 0, 0));
                rc.AddFrame(destWin, Color.Gray, 2);

                Life l = this.poe.Player.GetComponent<Life>();
                addLine(rc, "Health =" + l.CurHP + "/" + l.MaxHP);
                foreach (Poe.Buff b in l.Buffs)
                {
                    addLine(rc, b.Name + " (" + b.Timer.ToString() + ")");
                }
                addLine(rc, "----------------------------------");
                //Poe.Inventory inv = poe.Internal.IngameState.ServerData.PlayerInventories[InventoryIndex.PlayerInventory];
                addLine(rc, "Inventory Count : " + poe.Internal.IngameState.ServerData.PlayerInventories.InventoryCount);
            }


            //lines = lines;
           //if (l.Buffs.Count >3)

            // "frozen" --> of Heat
            // "shocked" --> "of Grounding"
            // "ignited" --> "of Dousing"
            // "corrupted_blood" --> "of Staunching"

            //Rect helpRect = new Rect(clientRect.X + 1, tooltipBotY, clientRect.W, i - tooltipBotY);
            //rc.AddBox(helpRect, Color.FromArgb(220, Color.Black));

            //rc.AddBox(rect, Color.FromArgb(bgAlpha, 1, 1, 1));
            //rc.AddFrame(frame, drawStyle.color, drawStyle.FrameWidth);


            //if (!Settings.GetBool("Tooltip") || !Settings.GetBool("Tooltip.ShowWeaponDps"))
            //    return;
            //Element uiHover = this.poe.Internal.IngameState.UIHover;
            //Poe.Entity Item = uiHover.AsObject<InventoryItemIcon>().Item;
            //if (Item.address == 0 || !Item.IsValid)
            //    return;
            //if (!Item.HasComponent<Weapon>()) // if its no weapon then leave
            //    return;
            //Weapon comp = Item.GetComponent<Weapon>();
            //List<ItemMod> expMods = Item.GetComponent<Mods>().ItemMods;


            //ItemStats stats = Item.GetComponent<Mods>().ItemStats;
            //float EleDPS = stats.GetStat(ItemStat.AverageElementalDamage);
            //float physDPS = stats.GetStat(ItemStat.PhysicalDPS);
            //float DPS = stats.GetStat(ItemStat.DPS);
            //Tooltip tooltip = uiHover.AsObject<InventoryItemIcon>().Tooltip;
            //if (tooltip == null)
            //    return;
            //Element childAtIndex1 = tooltip.GetChildAtIndex(0);
            //if (childAtIndex1 == null)
            //    return;
            //Element childAtIndex2 = childAtIndex1.GetChildAtIndex(1);
            //if (childAtIndex2 == null)
            //    return;
            //Rect clientRect = childAtIndex2.GetClientRect();
            //Rect headerRect = childAtIndex1.GetChildAtIndex(0).GetClientRect();

            ////int tooltipBotY=clientRect.Y + clientRect.H;
            ////int i = tooltipBotY;

            ////foreach (MaxRolls_Current item in this.mods)
            ////{
            ////    rc.AddTextWithHeight(new Vec2(clientRect.X, i), item.name, item.color, 8, DrawTextFormat.Left);
            ////    i += 20;
            ////    rc.AddTextWithHeight(new Vec2(clientRect.X + clientRect.W - 100, i), item.max, Color.White, 8, DrawTextFormat.Left);
            ////    rc.AddTextWithHeight(new Vec2(clientRect.X + 30, i), item.curr, Color.White, 8, DrawTextFormat.Left);
            ////    i += 20;
            ////    if (item.curr2 != null && item.max2 != null)
            ////    {
            ////        rc.AddTextWithHeight(new Vec2(clientRect.X + clientRect.W - 100, i), item.max2, Color.White, 8, DrawTextFormat.Left);
            ////        rc.AddTextWithHeight(new Vec2(clientRect.X + 30, i), item.curr2, Color.White, 8, DrawTextFormat.Left);
            ////        i += 20;
            ////    }
            ////}
            ////if (i > tooltipBotY)
            ////{
            ////    Rect helpRect = new Rect(clientRect.X + 1, tooltipBotY, clientRect.W, i - tooltipBotY);
            ////    rc.AddBox(helpRect, Color.FromArgb(220, Color.Black));
            ////}
        }
    }
}



