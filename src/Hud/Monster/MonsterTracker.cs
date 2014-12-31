using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.Icons;
using PoeHUD.Hud.Interfaces;
using PoeHUD.Hud.UI;
using PoeHUD.Models;
using PoeHUD.Models.Enums;
using PoeHUD.Poe.Components;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.Monster
{
	public class MonsterTracker : Plugin, IHudPluginWithMapIcons
	{
		private HashSet<int> alreadyAlertedOf;
		private Dictionary<EntityWrapper, string> alertTexts;

		private Dictionary<string, string> modAlerts;
		private Dictionary<string, string> typeAlerts;
		private readonly Dictionary<EntityWrapper, MapIcon> currentIcons = new Dictionary<EntityWrapper, MapIcon>();


	    public MonsterTracker(GameController gameController, Graphics graphics) : base(gameController, graphics)
	    {

            this.alreadyAlertedOf = new HashSet<int>();
            this.alertTexts = new Dictionary<EntityWrapper, string>();
            this.InitAlertStrings();
            this.GameController.Area.OnAreaChange += this.CurrentArea_OnAreaChange;

            currentIcons.Clear();
            foreach (EntityWrapper current in this.GameController.Entities)
            {
                this.OnEntityAdded(current);
            }
	    }

	
		protected override void OnEntityRemoved(EntityWrapper entity)
		{
			alertTexts.Remove(entity);
			currentIcons.Remove(entity);
		}

        protected override void OnEntityAdded(EntityWrapper entity)
		{
			if (!Settings.GetBool("MonsterTracker") || this.alertTexts.ContainsKey(entity))
			{
				return;
			}
			if (entity.IsAlive && entity.HasComponent<Poe.Components.Monster>())
			{
				currentIcons[entity] = GetMapIconForMonster(entity);
				string text = entity.Path;
				if (text.Contains('@'))
				{
					text = text.Split('@')[0];
				}
				if (this.typeAlerts.ContainsKey(text))
				{
					this.alertTexts.Add(entity, this.typeAlerts[text]);
					this.PlaySound(entity);
					return;
				}
				foreach (string current in entity.GetComponent<ObjectMagicProperties>().Mods)
				{
					if (this.modAlerts.ContainsKey(current))
					{
						this.alertTexts.Add(entity, this.modAlerts[current]);
						this.PlaySound(entity);
						break;
					}
				}
			}
		}
		private void PlaySound(EntityWrapper entity)
		{
			if (!Settings.GetBool("MonsterTracker.PlaySound"))
			{
				return;
			}
			if (!this.alreadyAlertedOf.Contains(entity.Id))
			{
				Sounds.DangerSound.Play();
				this.alreadyAlertedOf.Add(entity.Id);
			}
		}
		private void CurrentArea_OnAreaChange(AreaController area)
		{
			this.alreadyAlertedOf.Clear();
			this.alertTexts.Clear();
			currentIcons.Clear();
		}
		public override void Render(Dictionary<UiMountPoint, Vector2> mountPoints)
		{
			if (!Settings.GetBool("MonsterTracker.ShowText"))
			{
				return;
			}
			Rect rect = this.GameController.Window.ClientRect();
			int xScreenCenter = rect.W / 2 + rect.X;
			int yPos = rect.H / 10 + rect.Y;

			var playerPos = this.GameController.Player.GetComponent<Positioned>().GridPos;
			int fontSize = Settings.GetInt("MonsterTracker.ShowText.FontSize");
			bool first = true;
			var rectBackground = new RectangleF();

		    var groupedAlerts = alertTexts.Where(y => y.Key.IsAlive).Select(y =>
		    {
		        Vec2 delta = y.Key.GetComponent<Positioned>().GridPos - playerPos;
		        double phi;
		        var distance = delta.GetPolarCoordinates(out phi);
		        return new {Dic = y, Phi = phi, Distance = distance};
		    })
		        .OrderBy(y => y.Distance)
		        .GroupBy(y => y.Dic.Value)
		        .Select(y => new {Text = y.Key, Monster = y.First(), Count=y.Count()}).ToList();

            Color backgroundColor = new ColorBGRA(1, 1, 1, (byte)Settings.GetInt("MonsterTracker.ShowText.BgAlpha"));
            foreach (var group in groupedAlerts)
            {
                var uv = GetDirectionsUV(group.Monster.Phi, group.Monster.Distance);
                string text = String.Format("{0} {1}", group.Text, group.Count > 1 ? "(" + group.Count + ")" : string.Empty);
                var textSize = Graphics.DrawText(text, fontSize, new Vector2(xScreenCenter, yPos), Color.Red, FontDrawFlags.Center);

                rectBackground = new RectangleF(xScreenCenter - textSize.Width / 2 - 6, yPos, textSize.Width + 12, textSize.Height);
                rectBackground.X -= textSize.Height + 3;
                rectBackground.Width += textSize.Height;

                var rectDirection = new RectangleF(rectBackground.X + 3, rectBackground.Y, rectBackground.Height, rectBackground.Height);

                if (first) // vertical padding above
                {
                    rectBackground.Y -= 5;
                    rectBackground.Height += 5;
                    first = false;
                }
                Graphics.DrawBox(rectBackground, backgroundColor);
                Graphics.DrawImage("directions.png", rectDirection, uv, Color.Red);
                yPos += textSize.Height;
                
            }
			if (!first)  // vertical padding below
			{
				rectBackground.Y = rectBackground.Y + rectBackground.Height;
				rectBackground.Height = 5;
                Graphics.DrawBox(rectBackground, backgroundColor);
			}
		}

		public IEnumerable<MapIcon> GetIcons()
		{
			List<EntityWrapper> toRemove = new List<EntityWrapper>();
			foreach (KeyValuePair<EntityWrapper, MapIcon> kv in currentIcons)
			{
				if (kv.Value.IsEntityStillValid())
					yield return kv.Value;
				else
					toRemove.Add(kv.Key);
			}
			foreach (EntityWrapper wrapper in toRemove)
			{
				currentIcons.Remove(wrapper);
			}
		}


		private void InitAlertStrings()
		{
			this.modAlerts = LoadMonsterModAlerts();
			this.typeAlerts = LoadMonsterNameAlerts();
		}

		private MapIcon GetMapIconForMonster(EntityWrapper e)
		{
			if (!e.IsHostile)
				return new MapIconCreature(e, new HudTexture("monster_ally.png"), 6);

			switch (e.GetComponent<ObjectMagicProperties>().Rarity)
			{
				case MonsterRarity.White: return new MapIconCreature(e, new HudTexture("monster_enemy.png"), 6);
				case MonsterRarity.Magic: return new MapIconCreature(e, new HudTexture("monster_enemy_blue.png"), 8);
				case MonsterRarity.Rare: return new MapIconCreature(e, new HudTexture("monster_enemy_yellow.png"), 10);
				case MonsterRarity.Unique: return new MapIconCreature(e, new HudTexture("monster_enemy_orange.png"), 10);
			}
			return null;
		}

		private static Dictionary<string, string> LoadMonsterModAlerts()
		{
			var result = new Dictionary<string, string>();

			string[] lines = File.ReadAllLines("config/monster_mod_alerts.txt");
			foreach (string line in lines.Select(a => a.Trim()))
			{
				if (string.IsNullOrWhiteSpace(line) || line.IndexOf(',') < 0)
					continue;

				var parts = line.Split(new[] {','}, 2);
				result[parts[0].Trim()] = parts[1].Trim();
			}

			return result;
		}

		private static Dictionary<string, string> LoadMonsterNameAlerts()
		{
			var result = new Dictionary<string, string>();

			string[] lines = File.ReadAllLines("config/monster_name_alerts.txt");
			foreach (string line in lines.Select(a => a.Trim()))
			{
				if (string.IsNullOrWhiteSpace(line) || line.IndexOf(',') < 0)
					continue;

				var parts = line.Split(new[] { ',' }, 2);
				result[parts[0].Trim()] = parts[1].Trim();
			}

			return result;
		}
	}
}
