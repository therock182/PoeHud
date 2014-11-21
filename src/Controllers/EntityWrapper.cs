using System;
using System.Collections.Generic;
using System.Linq;
using PoeHUD.Framework;
using PoeHUD.Poe.EntityComponents;

namespace PoeHUD.Controllers
{
    public class EntityWrapper
    {
        private readonly Poe.Entity internalEntity;
        private readonly GameController root;
        private readonly Dictionary<string, int> components;
        private readonly int cachedId;
        public bool IsInList = true;

        public string Path { get; private set; }
        public bool IsValid { get { return internalEntity.IsValid && this.IsInList && this.cachedId == this.internalEntity.Id; } }
        public int Address { get { return internalEntity.Address; } }
        public int Id { get { return cachedId; } }
        public bool IsHostile { get { return internalEntity.IsHostile; } }
        public long LongId { get; private set; }
        public bool IsAlive { get { return GetComponent<Life>().CurHP > 0; } }
        public Vec3 Pos
        {
            get
            {
                var p = this.GetComponent<Positioned>();
                return new Vec3(p.X, p.Y, this.GetComponent<Render>().Z);
            }
        }
        public IEnumerable<EntityWrapper> Minions
        {
            get
            {
                return this.GetComponent<Actor>().Minions.Select(current => root.GetEntityById(current)).Where(byId => byId != null).ToList();
            }
        }
        public EntityWrapper(GameController Poe, Poe.Entity entity)
        {
            this.root = Poe;
            this.internalEntity = entity;
            this.components = this.internalEntity.GetComponents();
            this.Path = this.internalEntity.Path;
            this.cachedId = this.internalEntity.Id;
            this.LongId = this.internalEntity.LongId;
        }
        public EntityWrapper(GameController Poe, int address)
            : this(Poe, Poe.Game.GetObject<Poe.Entity>(address))
        {
        }
        public T GetComponent<T>() where T : Component, new()
        {
            string name = typeof(T).Name;
            return this.root.Game.GetObject<T>(this.components.ContainsKey(name) ? this.components[name] : 0);
        }
        public bool HasComponent<T>() where T : Component, new()
        {
            return this.components.ContainsKey(typeof(T).Name);
        }
        public void PrintComponents()
        {
            Console.WriteLine(this.internalEntity.Path + " " + this.internalEntity.Address.ToString("X"));
            foreach (KeyValuePair<string, int> current in this.components)
            {
                Console.WriteLine(current.Key + " " + current.Value.ToString("X"));
            }
        }
        public override bool Equals(object obj)
        {
            EntityWrapper entity = obj as EntityWrapper;
            return entity != null && entity.LongId == this.LongId;
        }
        public override int GetHashCode()
        {
            return this.LongId.GetHashCode();
        }

        public override string ToString()
        {
            return "EntityWrapper: " + Path;
        }
    }
}
