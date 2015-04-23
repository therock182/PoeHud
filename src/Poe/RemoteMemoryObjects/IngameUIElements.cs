using System;
using System.Collections.Generic;
using PoeHUD.Poe.Elements;
using PoeHUD.Poe.UI;
using PoeHUD.Poe.UI.Elements;

namespace PoeHUD.Poe.RemoteMemoryObjects
{
    public class IngameUIElements : RemoteMemoryObject
    {
        public Element Uknown0
        {
            get { return ReadObjectAt<Element>(0x40); }
        }

        public Element HpGlobe
        {
            get { return ReadObjectAt<Element>(0x44); }
        }

        public Element ManaGlobe
        {
            get { return ReadObjectAt<Element>(0x48); }
        }

        public Element Flasks
        {
            get { return ReadObjectAt<Element>(0x50); }
        }

        public Element XpBar
        {
            get { return ReadObjectAt<Element>(0x54); }
        }

        public Element MenuButton
        {
            get { return ReadObjectAt<Element>(0x58); }
        }

        public Element ShopButton
        {
            get { return ReadObjectAt<Element>(8 + 0x7C); }
        }

        public Element HideoutEditButton
        {
            get { return ReadObjectAt<Element>(0x88); }
        }

        public Element HideoutStashButton
        {
            get { return ReadObjectAt<Element>(0x8C); }
        }

        public Element Mouseposition
        {
            get { return ReadObjectAt<Element>(0xA4); }
        }

        public Element ActionButtons
        {
            get { return ReadObjectAt<Element>(0xA8); }
        }

        public Element Chat
        {
            get { return ReadObjectAt<Element>(16 + 0xD8); }
        }

        public Element QuestTracker
        {
            get { return ReadObjectAt<Element>(8 + 0xE8); }
        }

        public Element MtxInventory
        {
            get { return ReadObjectAt<Element>(8 + 0xEC); }
        }

        public Element MtxShop
        {
            get { return ReadObjectAt<Element>(8 + 0xF0); }
        }

        public Element InventoryPanel
        {
            get { return ReadObjectAt<Element>(20 + 0xF4); }
        }

        public Element StashPanel
        {
            get { return ReadObjectAt<Element>(16 + 0xF8); }
        }

        public Element SocialPanel
        {
            get { return ReadObjectAt<Element>(16 + 0x104); }
        }

        public Element TreePanel
        {
            get { return ReadObjectAt<Element>(20 + 0x108); }
        }

        public Element CharacterPanel
        {
            get { return ReadObjectAt<Element>(16 + 0x10C); }
        }

        public Element OptionsPanel
        {
            get { return ReadObjectAt<Element>(16 + 0x110); }
        }

        public Element AchievementsPanel
        {
            get { return ReadObjectAt<Element>(16 + 0x114); }
        }

        public Element WorldPanel
        {
            get { return ReadObjectAt<Element>(16 + 0x11C); }
        }

        public Map Map
        {
            get { return ReadObjectAt<Map>(24 + 0x11C); }
        }

        public IEnumerable<ItemsOnGroundLabelElement> ItemsOnGroundLabels
        {
            get
            {
                var itemsOnGroundLabelRoot = ReadObjectAt<ItemsOnGroundLabelElement>(24 + 0x120);
                return itemsOnGroundLabelRoot.Children;
            }
        }

        public List<HPbarElement> MonsterHpLabels
        {
            get
            {
                var monsterHpLabelsRoot = ReadObjectAt<HPbarElement>(8 + 0x124);
                return monsterHpLabelsRoot.Children;
            }
        }

        public Element Buffs
        {
            get { return ReadObjectAt<Element>(8 + 0x130); }
        }

        public Element Buffs2
        {
            get { return ReadObjectAt<Element>(8 + 0x18c); }
        }

        public Element OpenLeftPanel
        {
            get { return ReadObjectAt<Element>(24 + 0x154); }
        }

        public Element OpenRightPanel
        {
            get { return ReadObjectAt<Element>(24 + 0x158); }
        }

        public Element OpenNpcDialogPanel
        {
            get { return ReadObjectAt<Element>(8 + 0x160); }
        }

        public Element CreatureInfoPanel
        {
            get { return ReadObjectAt<Element>(8 + 0x184); }
        } // above, it shows name and hp

        public Element InstanceManagerPanel
        {
            get { return ReadObjectAt<Element>(8 + 0x198); }
        }

        public Element InstanceManagerPanel2
        {
            get { return ReadObjectAt<Element>(8 + 0x19C); }
        }

        // dunno what it is
        public Element SwitchingZoneInfo
        {
            get { return ReadObjectAt<Element>(0x1CC); }
        }

        public Element GemLvlUpPanel
        {
            get { return ReadObjectAt<Element>(36 + 0x1F8); }
        }

        public ItemOnGroundTooltip ItemOnGroundTooltip
        {
            get { return ReadObjectAt<ItemOnGroundTooltip>(36 + 0x208); }
        }
    }
}