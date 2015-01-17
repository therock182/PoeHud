using System;
using System.Collections.Generic;
using System.Linq;

using PoeHUD.Models.Enums;
using PoeHUD.Models.Interfaces;
using PoeHUD.Poe.Components;

namespace PoeHUD.Hud.Loot
{
    public class ItemUsefulProperties
    {
        private readonly bool isCurrency, isSkillGem, isRgb, isVaalFragment, isWeapon, isArmour, isFlask;

        private readonly int numSockets, numLinks, mapLevel, itemLevel, quality;

        private readonly ItemRarity rarity;

        private bool isCraftingBase;

        public ItemUsefulProperties(string name, IEntity item)
        {
            var mods = item.GetComponent<Mods>();
            var socks = item.GetComponent<Sockets>();
            Map map = item.HasComponent<Map>() ? item.GetComponent<Map>() : null;
            Quality qualityComponent = item.HasComponent<Quality>() ? item.GetComponent<Quality>() : null;

            Name = name;
            itemLevel = mods.ItemLevel;
            numLinks = socks.LargestLinkSize;
            numSockets = socks.NumberOfSockets;
            rarity = mods.ItemRarity;
            mapLevel = map == null ? 0 : 1;
            isCurrency = item.Path.Contains("Currency");
            isSkillGem = item.HasComponent<SkillGem>();
            quality = qualityComponent == null ? 0 : qualityComponent.ItemQuality;
            isRgb = socks.IsRGB;
            isWeapon = item.HasComponent<Weapon>();
            isArmour = item.HasComponent<Armour>();
            isFlask = item.HasComponent<Flask>();
            isVaalFragment = item.Path.Contains("VaalFragment");
        }

        public string Name { get; private set; }

        public AlertDrawStyle GetDrawStyle()
        {
            int iconIndex = -1;
            if (isRgb)
            {
                iconIndex = 1;
            }
            if (numSockets == 6)
            {
                iconIndex = 0;
            }
            if (isCraftingBase)
            {
                iconIndex = 2;
            }
            if (numLinks == 6)
            {
                iconIndex = 3;
            }

            return new AlertDrawStyle(rarity, isSkillGem, isCurrency)
            {
                FrameWidth = mapLevel > 0 || isVaalFragment ? 1 : 0,
                Text = string.Concat(quality > 0 ? "Superior " : String.Empty, Name),
                IconIndex = iconIndex
            };
        }

        public bool IsWorthAlertingPlayer(HashSet<string> currencyNames, ItemAlertSettings settings)
        {
            if (rarity == ItemRarity.Rare && settings.Rares)
            {
                return true;
            }
            if (rarity == ItemRarity.Unique && settings.Uniques)
            {
                return true;
            }
            if ((mapLevel > 0 || isVaalFragment) && settings.Maps)
            {
                return true;
            }
            if (numLinks >= settings.MinLinks)
            {
                return true;
            }
            if (isCurrency && settings.Currency)
            {
                if (currencyNames == null)
                {
                    if (!Name.Contains("Portal") && Name.Contains("Wisdom")) // TODO it's need to check
                    {
                        return true;
                    }
                }
                else if (currencyNames.Contains(Name))
                {
                    return true;
                }
            }

            if (isRgb && settings.Rgb)
            {
                return true;
            }
            if (settings.QualityItems.Enable)
            {
                QualityItemsSettings qualitySettings = settings.QualityItems;
                if (qualitySettings.Weapon.Enable && isWeapon && quality >= qualitySettings.Weapon.MinQuality
                    || qualitySettings.Armour.Enable && isArmour && quality >= qualitySettings.Armour.MinQuality
                    || qualitySettings.Flask.Enable && isFlask && quality >= qualitySettings.Flask.MinQuality
                    || qualitySettings.SkillGem.Enable && isSkillGem && quality >= qualitySettings.SkillGem.MinQuality)
                {
                    return true;
                }
            }
            return numSockets >= settings.MinSockets || isCraftingBase;
        }

        public void SetCraftingBase(CraftingBase craftingBase)
        {
            isCraftingBase = itemLevel >= craftingBase.MinItemLevel
                && quality >= craftingBase.MinQuality
                && (craftingBase.Rarities == null || craftingBase.Rarities.Contains(rarity));
        }
    }
}