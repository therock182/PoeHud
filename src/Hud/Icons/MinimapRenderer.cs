using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PoeHUD.ExileBot;
using PoeHUD.Framework;
using PoeHUD.Game;
using PoeHUD.Poe.EntityComponents;
using PoeHUD.Poe.UI;
using System.IO;

namespace PoeHUD.Hud.Icons
{
	public class MinimapRenderer : HUDPlugin
	{
		private const double InvSq2 = 0.7071067811;
		private List<MinimapIcon>[] icons;
		private Vec2 playerPos;
        Dictionary<string, string> minimpIcons;
        private HashSet<string> Mod = new HashSet<string>();

        public MinimapRenderer()
        {
            minimpIcons = new Dictionary<string, string>();
            minimpIcons.Add("ally", "monster_ally.png");
            minimpIcons.Add("white", "monster_enemy.png");
            minimpIcons.Add("magic", "monster_enemy_blue.png");
            minimpIcons.Add("rare", "monster_enemy_yellow.png");
            minimpIcons.Add("unique", "monster_enemy_orange.png");
            if (File.Exists("config/minimap_icons.txt"))
            {
                string[] lines = File.ReadAllLines("config/minimap_icons.txt");
                foreach (string line in lines.Select(a => a.Trim()))
                {
                    if (!line.StartsWith(";"))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;
                        var cols = line.Split(new[] { ',', ';' }, 2);
                        if (cols.Count() < 2)
                            continue;
                        if (string.IsNullOrWhiteSpace(cols[1]))
                            continue;
                        if (File.Exists("textures/" + cols[1].Trim()))
                            if (minimpIcons.ContainsKey(cols[0].Trim()))
                                minimpIcons[cols[0].Trim()] = cols[1].Trim();
                            else
                                minimpIcons.Add(cols[0].Trim(), cols[1].Trim());
                    }
                }
            }
            if (File.Exists("config/MonsterMods.txt"))
            {
                string[] lines = File.ReadAllLines("config/MonsterMods.txt");
                foreach (string line in lines.Select(a => a.Trim()))
                {
                    Mod.Add(line);
                }
            }
        }

		public override void OnEnable()
		{
			this.icons = new List<MinimapIcon>[Enum.GetValues(typeof(MinimapRenderPriority)).Length];
			for (int i = 0; i < this.icons.Length; i++)
			{
				this.icons[i] = new List<MinimapIcon>();
			}
			this.poe.EntityList.OnEntityAdded += this.EntityList_OnEntityAdded;
			foreach (Entity current in this.poe.Entities)
			{
				this.EntityList_OnEntityAdded(current);
			}



		}
		public override void OnDisable()
		{
		}
		private void EntityList_OnEntityAdded(Entity entity)
		{
			MinimapIcon icon = this.GetIcon(entity);
			if (icon != null)
			{
				this.AddIcon(icon);
			}
		}
		public override void Render(RenderingContext rc)
		{
			if (!this.poe.InGame || !Settings.GetBool("MinimapIcons"))
			{
				return;
			}
			this.playerPos = this.poe.Player.GetComponent<Positioned>().GridPos;
			Element smallMinimap = this.poe.Internal.IngameState.IngameUi.Minimap.SmallMinimap;
			float scale = 240f;
			Rect clientRect = smallMinimap.GetClientRect();
			Vec2 minimapCenter = new Vec2(clientRect.X + clientRect.W / 2, clientRect.Y + clientRect.H / 2);
			double diag = Math.Sqrt((double)(clientRect.W * clientRect.W + clientRect.H * clientRect.H)) / 2.0;
			List<MinimapIcon>[] array = this.icons;
			for (int i = 0; i < array.Length; i++)
			{
				List<MinimapIcon> list = array[i];
				list.RemoveAll((MinimapIcon x) => !x.Validate());
				foreach (MinimapIcon current in 
					from x in list
					where x.WantsToRender()
					select x)
				{
					Vec2 point = this.WorldToMinimap(current.WorldPosition, minimapCenter, diag, scale);
					current.RenderAt(rc, point);
				}
			}
		}
		private Vec2 WorldToMinimap(Vec2 world, Vec2 minimapCenter, double diag, float scale)
		{
            // Values according to 40 degree rotation of cartesian coordiantes, still doesn't seem right but closer
            float cosX = (float)((double)((float)(world.X - this.playerPos.X) / scale) * diag * Math.Cos((Math.PI / 180) * 40));
            float cosY = (float)((double)((float)(world.Y - this.playerPos.Y) / scale) * diag * Math.Cos((Math.PI / 180) * 40));
            float sinX = (float)((double)((float)(world.X - this.playerPos.X) / scale) * diag * Math.Sin((Math.PI / 180) * 40));
            float sinY = (float)((double)((float)(world.Y - this.playerPos.Y) / scale) * diag * Math.Sin((Math.PI / 180) * 40));
            // 2D rotation formulas not correct, but it's what appears to work?
            int x = (int)((float)minimapCenter.X + cosX - cosY);
            int y = (int)((float)minimapCenter.Y - (sinX + sinY));
			return new Vec2(x, y);
		}
		private MinimapIcon GetIcon(Entity e)
		{
			List<string> masters = new List<string> {
				"Metadata/NPC/Missions/Wild/Dex",
				"Metadata/NPC/Missions/Wild/DexInt",
				"Metadata/NPC/Missions/Wild/Int",
				"Metadata/NPC/Missions/Wild/Str",
				"Metadata/NPC/Missions/Wild/StrDex",
				"Metadata/NPC/Missions/Wild/StrDexInt",
				"Metadata/NPC/Missions/Wild/StrInt"
			};
			if (e.HasComponent<Poe.EntityComponents.NPC>() && masters.Contains(e.Path))
			{
				return new MasterMinimapIcon(e, minimpIcons["ally"], 10, MinimapRenderPriority.Strongbox);
			}
			if (e.HasComponent<Poe.EntityComponents.Monster>())
			{
				if (!e.IsHostile)
				{
                    return new MinionMinimapIcon(e, minimpIcons["ally"], 6, MinimapRenderPriority.BlueMonster);
				}
				switch (e.GetComponent<ObjectMagicProperties>().Rarity)
				{
				case MonsterRarity.White:
                        return new MonsterMinimapIcon(e, minimpIcons["white"], 6, MinimapRenderPriority.Monster);
				case MonsterRarity.Magic:
                        return new MonsterMinimapIcon(e, minimpIcons["magic"], 8, MinimapRenderPriority.BlueMonster);
				case MonsterRarity.Rare:
                        
                    if (e.HasComponent<ObjectMagicProperties>())
                    {
                        ObjectMagicProperties p = e.GetComponent<ObjectMagicProperties>();
#if DEBUG
                        foreach (string t in p.Mods)
                        {

                            //To fill an external file with all monster-mods. Learning while playing ....
                            if (!Mod.Contains(t))
                            {
                                File.WriteAllLines("config/MonsterMods.txt", Mod);
                                Mod.Add(t);
                            }
                        }
#endif
                        foreach (string t in p.Mods)
                            foreach (KeyValuePair<string, string> kv in minimpIcons)
                                if (kv.Key == t)
                                    return new MonsterMinimapIcon(e, kv.Value, 10, MinimapRenderPriority.RareMonster);
                    }
                    return new MonsterMinimapIcon(e, minimpIcons["rare"], 10, MinimapRenderPriority.RareMonster);

				case MonsterRarity.Unique:
                    return new MonsterMinimapIcon(e, minimpIcons["unique"], 10, MinimapRenderPriority.RareMonster);
				}
			}
			if (e.HasComponent<Chest>() && !e.GetComponent<Chest>().IsOpened)
			{
				if (!e.GetComponent<Chest>().IsStrongbox)
				{
					return new ChestMinimapIcon(e, "minimap_default_icon.png", 6);
				}
				switch (e.GetComponent<ObjectMagicProperties>().Rarity)
				{
				case MonsterRarity.White:
					return new StrongboxMinimapIcon(e, "strongbox.png", 16, Color.White);
				case MonsterRarity.Magic:
					return new StrongboxMinimapIcon(e, "strongbox.png", 16, Color.LightBlue);
				case MonsterRarity.Rare:
					return new StrongboxMinimapIcon(e, "strongbox.png", 16, Color.Yellow);
				case MonsterRarity.Unique:
					return new StrongboxMinimapIcon(e, "strongbox.png", 16, Color.Orange);
				}
			}
			return null;
		}
		public void AddIcon(MinimapIcon icon)
		{
			this.icons[(int)icon.RenderPriority].Add(icon);
		}
	}
}
