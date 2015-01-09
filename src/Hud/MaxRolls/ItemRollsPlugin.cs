using System;
using System.Collections.Generic;
using System.Linq;

using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.Interfaces;
using PoeHUD.Hud.UI;
using PoeHUD.Models.Enums;
using PoeHUD.Poe;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.Elements;
using PoeHUD.Poe.FilesInMemory;
using PoeHUD.Poe.RemoteMemoryObjects;
using PoeHUD.Poe.UI;
using PoeHUD.Poe.UI.Elements;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.MaxRolls
{
	public class ItemRollsPlugin : Plugin<ItemRollsSettings>
	{
		private Entity itemEntity;
        private List<RollValue> mods = new List<RollValue>();
        public ItemRollsPlugin(GameController gameController, Graphics graphics, ItemRollsSettings settings)
            : base(gameController, graphics, settings)
	    {
	    }
        
	    public override void Render()
		{
			if (!Settings.Enable)
				return;
			Element uiHover = this.GameController.Game.IngameState.UIHover;

			Element tooltip = uiHover.AsObject<InventoryItemIcon>().Tooltip;
			if (tooltip == null)
				return;
			Element childAtIndex1 = tooltip.GetChildAtIndex(0);
			if (childAtIndex1 == null)
				return;
			Element childAtIndex2 = childAtIndex1.GetChildAtIndex(1);
			if (childAtIndex2 == null)
				return;
			var clientRect = childAtIndex2.GetClientRect();

			Entity poeEntity = uiHover.AsObject<InventoryItemIcon>().Item;
			if (poeEntity.Address == 0 || !poeEntity.IsValid)
				return;
			if (this.itemEntity == null || this.itemEntity.Id != poeEntity.Id) {
				this.mods = new List<RollValue>();
				//List<Poe_ItemMod> impMods = poeEntity.GetComponent<Mods>().ImplicitMods;
				List<ItemMod> expMods = poeEntity.GetComponent<Mods>().ItemMods;
				int ilvl = poeEntity.GetComponent<Mods>().ItemLevel;
				foreach (ItemMod item in expMods)
					this.mods.Add(new RollValue(item, GameController.Files, ilvl));
				this.itemEntity = poeEntity;
			}
			float yPosTooltil = clientRect.Y + clientRect.Height + 5;
			float i = yPosTooltil+4;
			// Implicit mods
			//foreach (Poe_ItemMod item in impMods)
			//{
			//    rc.AddTextWithHeight(new Vec2(clientRect.X, yPos), item.Name, Color.Yellow, 9, DrawTextFormat.Left);
			//    rc.AddTextWithHeight(new Vec2(clientRect.X + clientRect.W - 10, yPos), item.Level.ToString(), Color.White, 6, DrawTextFormat.Left);
			//    yPos += 20;
			//}
            
	        foreach (RollValue item in this.mods)
			{
				i = DrawStatLine(item, clientRect, i);

				//if (item.curr2 != null && item.max2 != null)
				//{
				//	rc.AddTextWithHeight(new Vec2(clientRect.X + clientRect.W - 100, yPos), item.AllTiersRange2.ToString(), Color.White, 8, DrawTextFormat.Left);
				//	rc.AddTextWithHeight(new Vec2(clientRect.X + 30, yPos), item.curr2, Color.White, 8, DrawTextFormat.Left);
				//	yPos += 20;
				//}
			}
            if (i > yPosTooltil + 4)
			{
				var helpRect = new RectangleF(clientRect.X + 1, yPosTooltil, clientRect.Width, i - yPosTooltil);
                Color backgroundColor = Color.Black;
			    backgroundColor.A = 220;
                Graphics.DrawBox(helpRect, backgroundColor);
			}
		}


        private float DrawStatLine(RollValue item, RectangleF clientRect, float yPos)
		{
		    const float EPSILON = 0.001f;
            const int MARGIN_BOTTOM = 4;
			const int leftRuler = 50;

            float oldY = yPos;
			bool isUniqAffix = item.AffixType == ModsDat.ModType.Hidden;
			string prefix = item.AffixType == ModsDat.ModType.Prefix
				? "[P]"
				: item.AffixType == ModsDat.ModType.Suffix ? "[S]" : "[?]";
			if (!isUniqAffix)
			{
				if( item.CouldHaveTiers())
					prefix += " T" + item.Tier + " ";

                Graphics.DrawText(prefix, Settings.ModTextSize, new Vector2(clientRect.X + 5, yPos), Color.White);
                var textSize = Graphics.DrawText(item.AffixText, Settings.ModTextSize, new Vector2(clientRect.X + leftRuler, yPos), item.TextColor);
				yPos += textSize.Height;
			}

			for (int iStat = 0; iStat < 4; iStat++)
			{
				IntRange range = item.TheMod.StatRange[iStat];
				if(range.Min == 0 && range.Max == 0)
					continue;

				var theStat = item.TheMod.StatNames[iStat];
				int val = item.StatValue[iStat];
				float percents = range.GetPercentage(val);
				bool noSpread = !range.HasSpread();

				double hue = 120 * percents;
				if (noSpread) hue = 300;
				if (percents > 1) hue = 180;
				
				Color col = ColorUtils.ColorFromHsv(hue, 1, 1);

				string line2 = string.Format(noSpread ? "{0}" : "{0} [{1}]", theStat, range);

                Graphics.DrawText(line2, Settings.ModTextSize, new Vector2(clientRect.X + leftRuler, yPos), Color.White);

				string sValue = theStat.ValueToString(val);
                var txSize = Graphics.DrawText(sValue, Settings.ModTextSize, new Vector2(clientRect.X + leftRuler - 5, yPos), col, FontDrawFlags.Right);
				


				//if (!isUniqAffix)
				//	rc.AddTextWithHeight(new Vec2(clientRect.X + clientRect.W - 5, yPos), item.AllTiersRange[iStat].ToString(),
				//		Color.White, 8,
				//		DrawTextFormat.Right);
				yPos += txSize.Height;
			}
		    return Math.Abs(yPos - oldY) > EPSILON ? yPos + MARGIN_BOTTOM : oldY;
		}
	}
}