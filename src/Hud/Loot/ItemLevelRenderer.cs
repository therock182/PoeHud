using System.Collections.Generic;
using System.Drawing;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Poe;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.UI;
using PoeHUD.Poe.UI.Elements;
using SlimDX.Direct3D9;

namespace PoeHUD.Hud.Loot
{
	public class ItemLevelRenderer : HudPluginBase
	{
	    public ItemLevelRenderer(GameController gameController) : base(gameController)
	    {
	    }

	    public override void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints)
		{
			if (!Settings.GetBool("Tooltip") || !Settings.GetBool("Tooltip.ShowItemLevel"))
			{
				return;
			}
			Element uIHover = this.GameController.Game.IngameState.UIHover;
			Entity item = uIHover.AsObject<InventoryItemIcon>().Item;
			if (item.Address != 0 && item.IsValid)
			{
				Tooltip tooltip = uIHover.AsObject<InventoryItemIcon>().Tooltip;
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
				Rect clientRect = childAtIndex2.GetClientRect();
				rc.AddTextWithHeight(new Vec2(clientRect.X + 2, clientRect.Y + 2), item.GetComponent<Mods>().ItemLevel.ToString(), Color.White, 16, DrawTextFormat.Left);
			}
		}
	}
}
