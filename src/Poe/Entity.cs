using System.Collections.Generic;

namespace PoeHUD.Poe
{
    public sealed class Entity : RemoteMemoryObject
    {
        private int ComponentLookup
        {
            get { return M.ReadInt(Address, 88, 0); }
        }

        private int ComponentList
        {
            get { return M.ReadInt(Address + 4); }
        }

        public string Path
        {
            get { return M.ReadStringU(M.ReadInt(Address, 8)); }
        }

        public int Id
        {
            get { return M.ReadInt(Address + 24); }
        }

        public long LongId
        {
            get { return Id | Path.GetHashCode(); }
        }


        /// <summary>
        /// 0x65004D = "Me"(4 bytes) from word Metadata
        /// </summary>
        public bool IsValid
        {
            get { return M.ReadInt(Address, 8, 0) == 0x65004D; }
        }

        public bool IsHostile
        {
            get { return (M.ReadByte(Address + 29) & 1) == 0; }
        }

        public bool HasComponent<T>() where T : Component, new()
        {
            int addr;
            return (HasComponent<T>(out addr));
        }

        private bool HasComponent<T>(out int addr) where T : Component, new()
        {
            string name = typeof (T).Name;
            int componentLookup = ComponentLookup;
            addr = componentLookup;
            int num = 0;
            while (!M.ReadString(M.ReadInt(addr + 8)).Equals(name))
            {
                addr = M.ReadInt(addr);
                ++num;
                if (addr == componentLookup || addr == 0 || (addr == -1 || num >= 200))
                    return false;
            }
            return true;
        }


        public T GetComponent<T>() where T : Component, new()
        {
            int addr;
            if (HasComponent<T>(out addr))
                return ReadObject<T>(ComponentList + M.ReadInt(addr + 12)*4);
            return GetObject<T>(0);
        }

        public Dictionary<string, int> GetComponents()
        {
            var dictionary = new Dictionary<string, int>();
            int componentLookup = ComponentLookup;
            int addr = componentLookup;
            do
            {
                string key = M.ReadString(M.ReadInt(addr + 8));
                int num = M.ReadInt(ComponentList + M.ReadInt(addr + 12)*4);
                if (!dictionary.ContainsKey(key) && !string.IsNullOrWhiteSpace(key))
                    dictionary.Add(key, num);
                addr = M.ReadInt(addr);
            } while (addr != componentLookup && addr != 0 && addr != -1);
            return dictionary;
        }

        public override string ToString()
        {
            return Path;
        }
    }
}