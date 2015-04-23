using System;
using System.Collections.Generic;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.UI;

namespace PoeHUD.Poe.Elements
{
    public class ItemsOnGroundLabelElement : Element
    {
        private readonly Lazy<int> labelInfo;

        public ItemsOnGroundLabelElement()
        {
            labelInfo = new Lazy<int>(GetLabelInfo);
        }
        static readonly TimeSpan zero=new TimeSpan();

        public bool CanPickUp
        
        {
            get { return TimeLeft <= zero; }
        }

        public Entity ItemOnGround
        {
            get { return base.ReadObject<Entity>(Address + 0xC); }
        }

        public Element Label
        {
            get { return base.ReadObject<Element>(Address + 0x8); }
        }


        public TimeSpan TimeLeft
        {
            get
            {
                if (labelInfo.Value != 0)
                {
                    int futureTime = M.ReadInt(labelInfo.Value + 0x20);
                    return TimeSpan.FromMilliseconds(futureTime - Environment.TickCount);
                }
                return new TimeSpan();
            }
        }

        public TimeSpan MaxTimeForPickUp
        {
            get { return TimeSpan.FromMilliseconds(M.ReadInt(labelInfo.Value + 0x1C)); }
        }


        public new bool IsVisible
        {
            get { return Label.IsVisible; }
        }

        public new IEnumerable<ItemsOnGroundLabelElement> Children
        {
            get
            {

                int address = M.ReadInt(Address + 0x958);
                for (int nextAddress = M.ReadInt(address); nextAddress != address; nextAddress = M.ReadInt(nextAddress))
                {
                    yield return GetObject<ItemsOnGroundLabelElement>(nextAddress);
                }
            }
        }

        private int GetLabelInfo()
        {
            if (Label.Address != 0)
            {
                return ItemOnGround.GetComponent<Render>().UknownValue;
            }
            return 0;
        }
    }
}