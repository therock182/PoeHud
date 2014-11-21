using System.Collections.Generic;
using PoeHUD.Game.Enums;

namespace PoeHUD.Poe.EntityComponents
{
    public class ObjectMagicProperties : Component
    {
        public MonsterRarity Rarity
        {
            get
            {
                if (Address != 0)
                {
                    return (MonsterRarity) M.ReadInt(Address + 36);
                }
                return MonsterRarity.White;
            }
        }

        public List<string> Mods
        {
            get
            {
                if (Address == 0)
                {
                    return new List<string>();
                }
                int begin = M.ReadInt(Address + 56);
                int end = M.ReadInt(Address + 60);
                var list = new List<string>();
                if (begin == 0 || end == 0)
                {
                    return list;
                }
                for (int i = begin; i < end; i += 24)
                {
                    string mod = M.ReadStringU(M.ReadInt(i + 20, 1));
                    list.Add(mod);
                }
                return list;
            }
        }
    }
}