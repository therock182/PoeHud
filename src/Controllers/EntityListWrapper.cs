using System;
using System.Collections.Generic;
using PoeHUD.Poe;
using PoeHUD.Poe.UI;

namespace PoeHUD.Controllers
{
    public class EntityListWrapper
    {
        private readonly GameController gameController;
        private readonly HashSet<string> ignoredEntities;

        private Dictionary<int, EntityWrapper> entityCache;

        public EntityListWrapper(GameController gameController)
        {
            this.gameController = gameController;
            entityCache = new Dictionary<int, EntityWrapper>();
            ignoredEntities = new HashSet<string>();
            gameController.Area.OnAreaChange += AreaChanged;
        }

        public ICollection<EntityWrapper> Entities
        {
            get { return entityCache.Values; }
        }

        public EntityWrapper Player { get; private set; }
        public event Action<EntityWrapper> EntityAdded;

        public event Action<EntityWrapper> EntityRemoved;


        private void AreaChanged(AreaController area)
        {
            ignoredEntities.Clear();
            foreach (EntityWrapper current in entityCache.Values)
            {
                current.IsInList = false;
                if (EntityRemoved != null)
                {
                    EntityRemoved(current);
                }
            }
            entityCache.Clear();
            int address = gameController.Game.IngameState.Data.LocalPlayer.Address;
            if (Player == null || Player.Address != address)
            {
                Player = new EntityWrapper(gameController, address);
            }
        }

        public void RefreshState()
        {
            int address = gameController.Game.IngameState.Data.LocalPlayer.Address;
            if ((Player == null) || (Player.Address != address))
            {
                Player = new EntityWrapper(gameController, address);
            }

            Dictionary<int, Entity> currentEntities =
                gameController.Game.IngameState.Data.EntityList.EntitiesAsDictionary;
            var newCache = new Dictionary<int, EntityWrapper>();
            foreach (var kv in currentEntities)
            {
                int key = kv.Key;
                string item = kv.Value.Path + key;
                if (ignoredEntities.Contains(item))
                    continue;

                if (entityCache.ContainsKey(key) && entityCache[key].IsValid)
                {
                    newCache.Add(key, entityCache[key]);
                    entityCache[key].IsInList = true;
                    entityCache.Remove(key);
                    continue;
                }

                if (entityCache.ContainsKey(key))
                    entityCache.Remove(key);

                var entity = new EntityWrapper(gameController, kv.Value);
                if ((entity.Path.StartsWith("Metadata/Effects") || ((kv.Key & 0x80000000L) != 0L)) ||
                    entity.Path.StartsWith("Metadata/Monsters/Daemon"))
                {
                    ignoredEntities.Add(item);
                    continue;
                }

                if (!entity.IsValid)
                    continue;

                if (EntityAdded != null)
                {
                    EntityAdded(entity);
                }
                newCache.Add(key, entity);
            }

            foreach (EntityWrapper entity2 in entityCache.Values)
            {
                if (EntityRemoved != null)
                {
                    EntityRemoved(entity2);
                }
                entity2.IsInList = false;
            }
            entityCache = newCache;
        }

        public EntityWrapper GetEntityById(int id)
        {
            EntityWrapper result;
            return entityCache.TryGetValue(id, out result) ? result : null;
        }
        
        public EntityLabel GetLabelForEntity(EntityWrapper entity)
        {
            var hashSet = new HashSet<int>();
            int entityLabelMap = gameController.Game.IngameState.EntityLabelMap;
            int num = entityLabelMap;
            while (true)
            {
                hashSet.Add(num);
                if (gameController.Memory.ReadInt(num + 8) == entity.Address)
                {
                    break;
                }
                num = gameController.Memory.ReadInt(num);
                if (hashSet.Contains(num) || num == 0 || num == -1)
                {
                    return null;
                }
            }
            return gameController.Game.ReadObject<EntityLabel>(num + 12);
        }
    }
}