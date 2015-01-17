using PoeHUD.Models.Enums;

namespace PoeHUD.Hud.Loot
{
    public struct CraftingBase
    {
        public string Name { get; set; }

        public int MinItemLevel { get; set; }

        public int MinQuality { get; set; }

        public ItemRarity[] Rarities { get; set; }
    }
}