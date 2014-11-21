using PoeHUD.Poe.UI;

namespace PoeHUD.Poe
{
    public class IngameState : RemoteMemoryObject
    {
        public Camera Camera
        {
            get { return base.GetObject<Camera>(Address + 0x15B4 + Offsets.IgsOffset - Offsets.IgsDelta); }
        }

        public float CurrentZoomLevel
        {
            get { return M.ReadFloat(Address + 0x1694 + Offsets.IgsOffset - Offsets.IgsDelta); }
            set { M.WriteFloat(Address + 0x1694 + Offsets.IgsOffset - Offsets.IgsDelta, value); }
        }

        public IngameData Data
        {
            get { return base.ReadObject<IngameData>(Address + 0x138 + Offsets.IgsOffset); }
        }

        public bool InGame
        {
            get { return M.ReadInt(Address + 0x138 + Offsets.IgsOffset) != 0 && ServerData.IsInGame; }
        }

        public ServerData ServerData
        {
            get { return base.ReadObjectAt<ServerData>(0x13C + Offsets.IgsOffset); }
        }

        public IngameUIElements IngameUi
        {
            get { return base.ReadObjectAt<IngameUIElements>(0x5E8 + Offsets.IgsOffset); }
        }

        public Element UIRoot
        {
            get { return base.ReadObjectAt<Element>(0xC0C + Offsets.IgsOffset); }
        }

        public Element UIHover
        {
            get { return base.ReadObjectAt<Element>(0xC20 + Offsets.IgsOffset); }
        }

        public int EntityLabelMap
        {
            get { return M.ReadInt(Address + 68, 2528); }
        }
    }
}