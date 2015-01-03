using System;
using System.Collections.Generic;
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
	public class ItemRollsRenderer : Plugin
	{
		private Entity itemEntity;
        private List<RollValue> mods = new List<RollValue>();
        public ItemRollsRenderer(GameController gameController, Graphics graphics) : base(gameController, graphics)
	    {
	    }
        
	    public override void Render(Dictionary<UiMountPoint, Vector2> mountPoints)
		{
			if (!Settings.GetBool("Tooltip") || !Settings.GetBool("Tooltip.ShowItemMods"))
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
            
            //todo add to setting
	        if (poeEntity.HasComponent<Weapon>())
	        {
	            RenderWeaponStats(clientRect);
	        }
	        foreach (RollValue item in this.mods)
			{
				i = DrawStatLine(item, clientRect, i);

				i += 4;
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

        private static readonly Color[] eleCols = new[] { Color.White, HudSkin.DmgFireColor, HudSkin.DmgColdColor, HudSkin.DmgLightingColor, HudSkin.DmgChaosColor };


        private void RenderWeaponStats(RectangleF clientRect)
        {
            Weapon weapon = itemEntity.GetComponent<Weapon>();
            float aSpd = ((float)1000) / weapon.AttackTime;
            int cntDamages = Enum.GetValues(typeof(DamageType)).Length;
            float[] doubleDpsPerStat = new float[cntDamages];
            float physDmgMultiplier = 1;
            doubleDpsPerStat[(int)DamageType.Physical] = weapon.DamageMax + weapon.DamageMin;
            foreach (RollValue roll in mods)
            {
                for (int iStat = 0; iStat < 4; iStat++)
                {
                    IntRange range = roll.TheMod.StatRange[iStat];
                    if (range.Min == 0 && range.Max == 0)
                        continue;

                    StatsDat.StatRecord theStat = roll.TheMod.StatNames[iStat];
                    int val = roll.StatValue[iStat];
                    switch (theStat.Key)
                    {
                        case "physical_damage_+%":
                        case "local_physical_damage_+%":
                            physDmgMultiplier += val / 100f;
                            break;
                        case "local_attack_speed_+%":
                            aSpd *= (100f + val) / 100;
                            break;
                        case "local_minimum_added_physical_damage":
                        case "local_maximum_added_physical_damage":
                            doubleDpsPerStat[(int)DamageType.Physical] += val;
                            break;
                        case "local_minimum_added_fire_damage":
                        case "local_maximum_added_fire_damage":
                        case "unique_local_minimum_added_fire_damage_when_in_main_hand":
                        case "unique_local_maximum_added_fire_damage_when_in_main_hand":
                            doubleDpsPerStat[(int)DamageType.Fire] += val;
                            break;
                        case "local_minimum_added_cold_damage":
                        case "local_maximum_added_cold_damage":
                        case "unique_local_minimum_added_cold_damage_when_in_off_hand":
                        case "unique_local_maximum_added_cold_damage_when_in_off_hand":
                            doubleDpsPerStat[(int)DamageType.Cold] += val;
                            break;
                        case "local_minimum_added_lightning_damage":
                        case "local_maximum_added_lightning_damage":
                            doubleDpsPerStat[(int)DamageType.Lightning] += val;
                            break;
                        case "unique_local_minimum_added_chaos_damage_when_in_off_hand":
                        case "unique_local_maximum_added_chaos_damage_when_in_off_hand":
                        case "local_minimum_added_chaos_damage":
                        case "local_maximum_added_chaos_damage":
                            doubleDpsPerStat[(int)DamageType.Chaos] += val;
                            break;
           
                    }
                }
            }

            doubleDpsPerStat[(int)DamageType.Physical] *= physDmgMultiplier;
            var quality = itemEntity.GetComponent<Quality>().ItemQuality;
            if (quality > 0)
                doubleDpsPerStat[(int)DamageType.Physical] += (weapon.DamageMax + weapon.DamageMin) * quality / 100f;
            float pDps = doubleDpsPerStat[(int)DamageType.Physical] / 2 * aSpd;

            float eDps = 0;
            int firstEmg = 0;
            Color eDpsColor = Color.White;

            for (int i = 1; i < cntDamages; i++)
            {
                eDps += doubleDpsPerStat[i] / 2 * aSpd;
                if (doubleDpsPerStat[i] > 0)
                {
                    if (firstEmg == 0)
                    {
                        firstEmg = i;
                        eDpsColor = eleCols[i];
                    }
                    else
                    {
                        eDpsColor = Color.DarkViolet;
                    }
                }
            }
            //todo need to put in setting
            const int OffsetInnerX = 2;
            const int OffsetInnerY = 1;
            const int DpsFontSize = 12;
            const int DpSNameFontSize = 8;

            Size2 sz = new Size2();
            if (pDps > 0)
                sz = Graphics.DrawText(pDps.ToString("#.#"), DpsFontSize, new Vector2(clientRect.X + clientRect.Width - OffsetInnerX, clientRect.Y + OffsetInnerY), Color.White, FontDrawFlags.Right);
            Size2 sz2 = new Size2();
            if (eDps > 0)
                sz2 = Graphics.DrawText(eDps.ToString("#.#"), DpsFontSize, new Vector2(clientRect.X + clientRect.Width - OffsetInnerX, clientRect.Y + OffsetInnerY + sz.Height), eDpsColor, FontDrawFlags.Right);
            Graphics.DrawText("DPS", DpSNameFontSize, new Vector2(clientRect.X + clientRect.Width - OffsetInnerX, clientRect.Y + OffsetInnerY + sz.Height + sz2.Height), Color.White, FontDrawFlags.Right);
        }

		private float DrawStatLine(RollValue item, RectangleF clientRect, float yPos)
		{
			const int leftRuler = 50;

			bool isUniqAffix = item.AffixType == ModsDat.ModType.Hidden;
			string prefix = item.AffixType == ModsDat.ModType.Prefix
				? "[P]"
				: item.AffixType == ModsDat.ModType.Suffix ? "[S]" : "[?]";
			if (!isUniqAffix)
			{
				if( item.CouldHaveTiers())
					prefix += " T" + item.Tier + " ";

                Graphics.DrawText(prefix, 8, new Vector2(clientRect.X + 5, yPos), Color.White);
                var textSize = Graphics.DrawText(item.AffixText, 8, new Vector2(clientRect.X + leftRuler, yPos), item.TextColor);
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

                Graphics.DrawText(line2, 8, new Vector2(clientRect.X + leftRuler, yPos), Color.White);

				string sValue = theStat.ValueToString(val);
                var txSize = Graphics.DrawText(sValue, 8, new Vector2(clientRect.X + leftRuler - 5, yPos), col, FontDrawFlags.Right);
				


				//if (!isUniqAffix)
				//	rc.AddTextWithHeight(new Vec2(clientRect.X + clientRect.W - 5, yPos), item.AllTiersRange[iStat].ToString(),
				//		Color.White, 8,
				//		DrawTextFormat.Right);
				yPos += txSize.Height;
			}
			return yPos;
		}
	}
}