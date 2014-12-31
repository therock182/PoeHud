using System.Collections.Generic;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.UI;
using PoeHUD.Poe;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.Elements;
using PoeHUD.Poe.UI;
using PoeHUD.Poe.UI.Elements;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.Loot
{
	public class ItemLevelRenderer : Plugin
	{
	    public ItemLevelRenderer(GameController gameController, Graphics graphics) : base(gameController, graphics)
	    {
	    }

	    public override void Render(Dictionary<UiMountPoint, Vector2> mountPoints)
		{
			if (!Settings.GetBool("Tooltip") || !Settings.GetBool("Tooltip.ShowItemLevel"))
			{
				return;
			}
			Element uIHover = this.GameController.Game.IngameState.UIHover;
			Entity item = uIHover.AsObject<InventoryItemIcon>().Item;
			if (item.Address != 0 && item.IsValid)
			{
				Element tooltip = uIHover.AsObject<InventoryItemIcon>().Tooltip;
				if (tooltip == null)
				{
					return;
				}
				Element childAtIndex = tooltip.GetChildAtIndex(0);
				if (childAtIndex == null)
				{
					return;
				}
				Element childAtIndex2 = childAtIndex.GetChildAtIndex(1);
				if (childAtIndex2 == null)
				{
					return;
				}
				var clientRect = childAtIndex2.GetClientRect();
                Graphics.DrawText(item.GetComponent<Mods>().ItemLevel.ToString(), 16, new Vector2(clientRect.X + 2, clientRect.Y + 2), Color.White);
			}
		}
	}
}
