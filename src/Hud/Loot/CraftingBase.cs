using PoeHUD.Game;
using PoeHUD.Game.Enums;

namespace PoeHUD.Hud.Loot
{
	public struct CraftingBase
	{
		public string Name;
		public int MinItemLevel;
		public int MinQuality;
		public ItemRarity[] Rarities;
	}
}