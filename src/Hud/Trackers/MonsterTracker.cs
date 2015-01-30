using System;
using System.Collections.Generic;
using System.Linq;

using PoeHUD.Controllers;
using PoeHUD.Framework.Helpers;
using PoeHUD.Hud.UI;
using PoeHUD.Models;
using PoeHUD.Models.Enums;
using PoeHUD.Models.Interfaces;
using PoeHUD.Poe.Components;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.Trackers
{
    public class MonsterTracker : PluginWithMapIcons<MonsterTrackerSettings>
    {
        private readonly HashSet<int> alreadyAlertedOf;

        private readonly Dictionary<EntityWrapper, string> alertTexts;

        private readonly Dictionary<MonsterRarity, Func<EntityWrapper, CreatureMapIcon>> iconCreators;

        private readonly Dictionary<string, string> modAlerts, typeAlerts;

        public MonsterTracker(GameController gameController, Graphics graphics, MonsterTrackerSettings settings)
            : base(gameController, graphics, settings)
        {
            alreadyAlertedOf = new HashSet<int>();
            alertTexts = new Dictionary<EntityWrapper, string>();
            modAlerts = LoadConfig("config/monster_mod_alerts.txt");
            typeAlerts = LoadConfig("config/monster_name_alerts.txt");
            Func<bool> monsterSettings = () => Settings.Monsters;
            iconCreators = new Dictionary<MonsterRarity, Func<EntityWrapper, CreatureMapIcon>>
            {
                { MonsterRarity.White, e => new CreatureMapIcon(e, "monster_enemy.png", monsterSettings, 6) },
                { MonsterRarity.Magic, e => new CreatureMapIcon(e, "monster_enemy_blue.png", monsterSettings, 8) },
                { MonsterRarity.Rare, e => new CreatureMapIcon(e, "monster_enemy_yellow.png", monsterSettings, 10) },
                { MonsterRarity.Unique, e => new CreatureMapIcon(e, "monster_enemy_orange.png", monsterSettings, 10) },
            };
            GameController.Area.OnAreaChange += area =>
            {
                alreadyAlertedOf.Clear();
                alertTexts.Clear();
            };
        }

        public override void Render()
        {
            if (!Settings.Enable || !Settings.ShowText)
            {
                return;
            }

            RectangleF rect = GameController.Window.GetWindowRectangle();
            float xPos = rect.Width * Settings.TextPositionX / 100 + rect.X;
            float yPos = rect.Height * Settings.TextPositionY / 100 + rect.Y;

            Vector2 playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
            bool first = true;
            var rectBackground = new RectangleF();

            var groupedAlerts = alertTexts.Where(y => y.Key.IsAlive).Select(y =>
            {
                Vector2 delta = y.Key.GetComponent<Positioned>().GridPos - playerPos;
                double phi;
                double distance = delta.GetPolarCoordinates(out phi);
                return new { Dic = y, Phi = phi, Distance = distance };
            })
                .OrderBy(y => y.Distance)
                .GroupBy(y => y.Dic.Value)
                .Select(y => new { Text = y.Key, Monster = y.First(), Count = y.Count() }).ToList();

            foreach (var group in groupedAlerts)
            {
                RectangleF uv = GetDirectionsUV(group.Monster.Phi, group.Monster.Distance);
                string text = String.Format("{0} {1}", group.Text, group.Count > 1 ? "(" + group.Count + ")" : string.Empty);
                Size2 textSize = Graphics.DrawText(text, Settings.TextSize, new Vector2(xPos, yPos), Color.Red,
                    FontDrawFlags.Center);

                rectBackground = new RectangleF(xPos - textSize.Width / 2f - 6, yPos, textSize.Width + 12,
                    textSize.Height);
                rectBackground.X -= textSize.Height + 3;
                rectBackground.Width += textSize.Height;

                var rectDirection = new RectangleF(rectBackground.X + 3, rectBackground.Y, rectBackground.Height,
                    rectBackground.Height);

                if (first) // vertical padding above
                {
                    rectBackground.Y -= 5;
                    rectBackground.Height += 5;
                    first = false;
                }
                Graphics.DrawBox(rectBackground, Settings.BackgroundColor);
                Graphics.DrawImage("directions.png", rectDirection, uv, Color.Red);
                yPos += textSize.Height;
            }
            if (!first) // vertical padding below
            {
                rectBackground.Y = rectBackground.Y + rectBackground.Height;
                rectBackground.Height = 5;
                Graphics.DrawBox(rectBackground, Settings.BackgroundColor);
            }
        }

        protected override void OnEntityAdded(EntityWrapper entity)
        {
            if (!Settings.Enable || alertTexts.ContainsKey(entity))
            {
                return;
            }
            if (entity.IsAlive && entity.HasComponent<Monster>())
            {
                MapIcon mapIcon = GetMapIconForMonster(entity);
                if (mapIcon != null)
                {
                    CurrentIcons[entity] = mapIcon;
                }
                string text = entity.Path;
                if (text.Contains('@'))
                {
                    text = text.Split('@')[0];
                }
                if (typeAlerts.ContainsKey(text))
                {
                    alertTexts.Add(entity, typeAlerts[text]);
                    PlaySound(entity);
                    return;
                }
                string modAlert = entity.GetComponent<ObjectMagicProperties>().Mods.FirstOrDefault(x => modAlerts.ContainsKey(x));
                if (modAlert != null)
                {
                    alertTexts.Add(entity, modAlerts[modAlert]);
                    PlaySound(entity);
                }
            }
        }

        protected override void OnEntityRemoved(EntityWrapper entity)
        {
            base.OnEntityRemoved(entity);
            alertTexts.Remove(entity);
        }

        private MapIcon GetMapIconForMonster(EntityWrapper entity)
        {
            if (!entity.IsHostile)
            {
                return new CreatureMapIcon(entity, "monster_ally.png", () => Settings.Minions, 6);
            }

            MonsterRarity monsterRarity = entity.GetComponent<ObjectMagicProperties>().Rarity;
            Func<EntityWrapper, CreatureMapIcon> iconCreator;
            return iconCreators.TryGetValue(monsterRarity, out iconCreator) ? iconCreator(entity) : null;
        }

        private void PlaySound(IEntity entity)
        {
            if (Settings.PlaySound && !alreadyAlertedOf.Contains(entity.Id))
            {
                Sounds.DangerSound.Play();
                alreadyAlertedOf.Add(entity.Id);
            }
        }
    }
}