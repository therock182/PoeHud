using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeHUD.Poe.UI;

namespace PoeHUD.Poe.Elements
{
    public class ItemOnGroundTooltip : Element
    {
        public Entity Item
        {
            get
            {
                var address = M.ReadInt(Address + OffsetBuffers, 0, 0x8EC, 0x904);
                var entity = GetObject<Entity>(address);
                return entity;
            }
        }

        public Element ToolTip
        {
            get
            {
                return GetChildAtIndex(0);
            }
        }
    }
}
