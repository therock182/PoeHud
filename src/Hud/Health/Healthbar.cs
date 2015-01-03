using PoeHUD.Models;
using PoeHUD.Models.Enums;
using PoeHUD.Poe.Components;
using SharpDX;

namespace PoeHUD.Hud.Health
{
    internal class Healthbar
    {
        private readonly bool isHostile;
        public readonly EntityWrapper Entity;
        public readonly bool IsValid;
        public RenderPrio Prio;
        public readonly AllyUnitSettings Settings;


        public Healthbar(EntityWrapper entity, HealthBarSettings settings)
        {
            Entity = entity;
            if (entity.HasComponent<Player>())
            {
                Prio = RenderPrio.Player;
                Settings = settings.Players;
                IsValid = true;
            }
            if (entity.HasComponent<Poe.Components.Monster>())
            {
                IsValid = true;
                if (entity.IsHostile)
                {
                    isHostile = true;
                    switch (entity.GetComponent<ObjectMagicProperties>().Rarity)
                    {
                        case MonsterRarity.White:
                            Prio = RenderPrio.Normal;
                            Settings = settings.NormalEnemy;
                            break;
                        case MonsterRarity.Magic:
                            Prio = RenderPrio.Magic;
                            Settings = settings.MagicEnemy;
                            break;
                        case MonsterRarity.Rare:
                            Settings = settings.RareEnemy;
                            Prio = RenderPrio.Rare;
                            break;
                        case MonsterRarity.Unique:
                            Settings = settings.UniqueEnemy;
                            Prio = RenderPrio.Unique;
                            break;
                    }
                }
                else
                {
                    Prio = RenderPrio.Minion;
                    Settings = settings.Minions;
                }
            }
        }

        public bool IsShow(bool showEnemy)
        {
            return !isHostile ? Settings.Enable.Value : Settings.Enable && showEnemy && isHostile;
        }
    }
}