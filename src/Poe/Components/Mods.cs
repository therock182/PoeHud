using System;
using System.Collections.Generic;
using System.Linq;
using PoeHUD.Controllers;
using PoeHUD.Models;
using PoeHUD.Models.Enums;
using PoeHUD.Poe.RemoteMemoryObjects;

namespace PoeHUD.Poe.Components
{
    public class Mods : Component
    {
        public ItemRarity ItemRarity
        {
            get
            {
                if (Address != 0)
                {
                    return (ItemRarity) M.ReadInt(Address + 0x68);
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
                    return M.ReadInt(Address + 0x10C);
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
                    return M.ReadStringU(M.ReadInt(Address + 12, new[] { 4, 4 }));
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
                var implicitMods = GetMods(0x6C, 0x70);
                var explicitMods = GetMods(0x7C, 0x80);
                return implicitMods.Concat(explicitMods).ToList();
            }
        }

        private List<ItemMod> GetMods(int startOffset, int endOffset)
        {
            var list = new List<ItemMod>();
            if (Address == 0)
                return list;

            int begin = M.ReadInt(Address + startOffset);
            int end = M.ReadInt(Address + endOffset);
            int count = (end - begin) / 24;
            if (count > 12)
                return list;

            for (int i = begin; i < end; i += 24)
                list.Add(base.GetObject<ItemMod>(i));

            return list;
        }

    }
}