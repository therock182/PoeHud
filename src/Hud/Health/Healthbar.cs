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
        public readonly string Settings;


        public Healthbar(EntityWrapper entity)
        {
            Entity = entity;
            if (entity.HasComponent<Player>())
            {
                Prio = RenderPrio.Player;
                Settings = "Healthbars.Players";
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
                            Settings = "Healthbars.Enemies.Normal";
                            break;
                        case MonsterRarity.Magic:
                            Prio = RenderPrio.Magic;
                            Settings = "Healthbars.Enemies.Magic";
                            break;
                        case MonsterRarity.Rare:
                            Settings = "Healthbars.Enemies.Rare";
                            Prio = RenderPrio.Rare;
                            break;
                        case MonsterRarity.Unique:
                            Settings = "Healthbars.Enemies.Unique";
                            Prio = RenderPrio.Unique;
                            break;
                    }
                }
                else
                {
                    Prio = RenderPrio.Minion;
                    Settings = "Healthbars.Minions";
                }
            }
        }

        public Bool Show
        {
            get
            {
                bool showEnemies = Hud.Settings.GetBool("Healthbars.Enemies") && isHostile;
                return !isHostile ? Hud.Settings.GetBool(Settings) : Hud.Settings.GetBool(Settings) && showEnemies;
            }
        }
    }
}