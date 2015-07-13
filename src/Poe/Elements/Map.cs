namespace PoeHUD.Poe.UI.Elements
{
    public class Map : Element
    {

        public float ShiftX
        {
            get
            {
                return M.ReadFloat(M.ReadInt(Address+0x168 + OffsetBuffers) + 0x8F8);
            }
        }

        public float ShiftY
        {
            get
            {
                return M.ReadFloat(M.ReadInt(Address+0x168 + OffsetBuffers) + 0x8FC);
            }
        }

        public Element SmallMinimap
        {
            get { return base.ReadObjectAt<Element>(0x16C + OffsetBuffers); }
        }

        // when this is visible, draw on large map
        public Element MapProperties
        {
            get { return base.ReadObjectAt<Element>(0x170 + OffsetBuffers); }
        }


        public Element LargeMap
        {
            get { return base.ReadObjectAt<Element>(0x168 + OffsetBuffers); }
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