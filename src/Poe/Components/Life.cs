using System.Collections.Generic;
using PoeHUD.Poe.RemoteMemoryObjects;

namespace PoeHUD.Poe.Components
{
    public class Life : Component
    {
        public int MaxHP
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 80);
                }
                return 1;
            }
        }

        public int CurHP
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 84);
                }
                return 1;
            }
        }

        public int ReservedHP
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 92);
                }
                return 0;
            }
        }

        public int MaxMana
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 116);
                }
                return 1;
            }
        }

        public int CurMana
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 120);
                }
                return 1;
            }
        }

        public int ReservedMana
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 128);
                }
                return 0;
            }
        }

        public int MaxES
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 152);
                }
                return 0;
            }
        }

        public int CurES
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 156);
                }
                return 0;
            }
        }

        public float HPPercentage
        {
            get { return CurHP/(float) (MaxHP - ReservedHP); }
        }

        public float MPPercentage
        {
            get { return CurMana/(float) (MaxMana - ReservedMana); }
        }

        public float ESPercentage
        {
            get
            {
                if (MaxES != 0)
                {
                    return CurES/(float) MaxES;
                }
                return 0f;
            }
        }

        public bool CorpseUsable
        {
            get { return M.ReadBytes(Address + 212, 1)[0] == 1; }
        }

        public List<Buff> Buffs
        {
            get
            {
                var list = new List<Buff>();
                int start = M.ReadInt(Address + 184);
                int end = M.ReadInt(Address + 188);
                int count = (end - start)/4;
                if (count <= 0 || count > 32)
                {
                    return list;
                }
                for (int i = 0; i < count; i++)
                {
                    list.Add(base.ReadObject<Buff>(M.ReadInt(start + i*4) + 4));
                }
                return list;
            }
        }

        public bool HasBuff(string buff)
        {
            return Buffs.Exists((Buff x) => x.Name == buff);
        }
    }
}