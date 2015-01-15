using System.Collections.Generic;

using PoeHUD.Models.Enums;

using SharpDX;

namespace PoeHUD.Hud.Loot
{
    public sealed class AlertDrawStyle
    {
        private static readonly Dictionary<ItemRarity, Color> colors = new Dictionary<ItemRarity, Color>
        {
            { ItemRarity.White, Color.White },
            { ItemRarity.Magic, HudSkin.MagicColor },
            { ItemRarity.Rare, HudSkin.RareColor },
            { ItemRarity.Unique, HudSkin.UniqueColor },
        };

        public AlertDrawStyle(ItemRarity rarity, bool isSkillGem, bool isCurrency)
        {
            if (isSkillGem)
            {
                Color = HudSkin.SkillGemColor;
            }
            else if (isCurrency)
            {
                Color = HudSkin.CurrencyColor;
            }
            else
            {
                Color color;
                Color = colors.TryGetValue(rarity, out color) ? color : Color.White;
            }
        }

        public Color Color { get; private set; }

        public int FrameWidth { get; set; }

        public string Text { get; set; }

        public int IconIndex { get; set; }
    }
}