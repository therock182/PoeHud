using PoeHUD.Framework;
using PoeHUD.Hud.Loot;
using PoeHUD.Hud.UI;
using PoeHUD.Models.Enums;

using SharpDX;

namespace PoeHUD.Hud
{
	public class HudTexture
	{
		public string TextureFile;
		public Color TintColor;

		public HudTexture(string fileName) : this(fileName, Color.White) { }

		public HudTexture(string fileName, MonsterRarity rarity) : this(fileName, Color.White)
		{
			switch (rarity)
			{
				case MonsterRarity.Magic: TintColor = HudSkin.MagicColor; break;
				case MonsterRarity.Rare: TintColor = HudSkin.RareColor; break;
				case MonsterRarity.Unique: TintColor = HudSkin.UniqueColor; break;
			}
		}

		public HudTexture(string fileName, Color tintColor) 
		{
			this.TextureFile = fileName;
			TintColor = tintColor;
		}


		public void DrawAt(Graphics graphics, RectangleF rect)
		{
            graphics.DrawImage(this.TextureFile, rect, TintColor);
		}
	}
}
