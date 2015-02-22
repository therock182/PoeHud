namespace PoeHUD.Poe.UI.Elements
{
    public class Map : Element
    {

        public float ShiftX
        {
            get
            {
                return M.ReadFloat(M.ReadInt(Address+0x164 + OffsetBuffers) + 0x970);
            }
        }

        public float ShiftY
        {
            get
            {
                return M.ReadFloat(M.ReadInt(Address+0x164 + OffsetBuffers) + 0x974);
            }
        }

        public Element SmallMinimap
        {
            get { return base.ReadObjectAt<Element>(0x168 + OffsetBuffers); }
        }

        // when this is visible, draw on large map
        public Element MapProperties
        {
            get { return base.ReadObjectAt<Element>(0x170 + OffsetBuffers); }
        }

        public Element OrangeWords
        {
            get { return base.ReadObjectAt<Element>(0x174 + OffsetBuffers); }
        }

        public Element BlueWords
        {
            get { return base.ReadObjectAt<Element>(0x18c + OffsetBuffers); }
        }
    }
}