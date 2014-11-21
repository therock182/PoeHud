using System.Collections.Generic;
using PoeHUD.Controllers;
using PoeHUD.Game.Enums;

namespace PoeHUD.Poe.EntityComponents
{
    public class Mods : Component
    {
        public ItemRarity ItemRarity
        {
            get
            {
                if (Address != 0)
                {
                    return (ItemRarity) M.ReadInt(Address + 48);
                }
                return ItemRarity.White;
            }
        }

        public int ItemLevel
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 212);
                }
                return 1;
            }
        }

        public string UniqueName
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadStringU(M.ReadInt(Address + 12, new[] {4, 4}));
                }
                return "";
            }
        }

        public ItemStats ItemStats
        {
            get { return new ItemStats(base.Owner); }
        }

        public List<ItemMod> ItemMods
        {
            get
            {
                var list = new List<ItemMod>();
                if (Address == 0)
                    return list;

                int i = M.ReadInt(Address + 68);
                int end = M.ReadInt(Address + 72);
                int num2 = (end - i)/24;
                if (num2 > 12)
                    return list;

                for (; i < end; i += 24)
                    list.Add(base.GetObject<ItemMod>(i));

                return list;
            }
        }

        public List<ItemMod> ImplicitMods
        {
            get
            {
                var list = new List<ItemMod>();
                if (Address == 0)
                    return list;

                int i = M.ReadInt(Address + 52);
                int end = M.ReadInt(Address + 56);
                int num2 = (end - i)/24;
                if (num2 > 100 || num2 <= 0)
                    return list;

                for (; i < end; i += 24)
                    list.Add(base.GetObject<ItemMod>(i));

                return list;
            }
        }
    }
}