using System;
using System.Collections.Generic;
using PoeHUD.Controllers;
using PoeHUD.Poe;
using PoeHUD.Poe.UI.Elements;

namespace PoeHUD.Models
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
            gameController.Area.OnAreaChange += OnAreaChanged;
        }

        public ICollection<EntityWrapper> Entities
        {
            get { return entityCache.Values; }
        }

        public EntityWrapper Player { get; private set; }
        public event Action<EntityWrapper> OnEntityAdded;

        public event Action<EntityWrapper> OnEntityRemoved;


        private void OnAreaChanged(AreaController area)
        {
            ignoredEntities.Clear();
            foreach (EntityWrapper current in entityCache.Values)
            {
                current.IsInList = false;
                if (OnEntityRemoved != null)
                {
                    OnEntityRemoved(current);
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
            Dictionary<int, Entity> newEntities = gameController.Game.IngameState.Data.EntityList.EntitiesAsDictionary;
            var newCache = new Dictionary<int, EntityWrapper>();
            foreach (var keyEntity in newEntities)
            {
                int key = keyEntity.Key;
                string uniqueEntityName = keyEntity.Value.Path + key;
                if (ignoredEntities.Contains(uniqueEntityName))
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

                var entity = new EntityWrapper(gameController, keyEntity.Value);
                if ((entity.Path.StartsWith("Metadata/Effects") || ((keyEntity.Key & 0x80000000L) != 0L)) || entity.Path.StartsWith("Metadata/Monsters/Daemon"))
                {
                    ignoredEntities.Add(uniqueEntityName);
                    continue;
                }

                if (!entity.IsValid)
                    continue;

                if (OnEntityAdded != null)
                {
                    OnEntityAdded(entity);
                }
                newCache.Add(key, entity);
            }

            foreach (EntityWrapper entity2 in entityCache.Values)
            {
                if (OnEntityRemoved != null)
                {
                    OnEntityRemoved(entity2);
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