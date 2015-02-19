using System;
using PoeHUD.Poe.UI;

namespace PoeHUD.Poe.RemoteMemoryObjects
{
    public class IngameState : RemoteMemoryObject
    {
        public Camera Camera
        {
            get { return base.GetObject<Camera>(Address + 0x15B8 + Offsets.IgsOffsetDelta); }
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

        /// <summary>
        /// Latency in ms
        /// </summary>
        public float CurLatency
        {
            get { return M.ReadFloat(Address + 0xc8c); }
        }

        /// <summary>
        /// Frame time in ms
        /// </summary>
        public float CurFrameTime
        {
            get { return M.ReadFloat(Address + 0x10f4); }
        }


        public float CurFps
        {
            get { return M.ReadFloat(Address + 0x1370); }
        }

        /// <summary>
        /// How much time client is running
        /// </summary>
        public TimeSpan TimeInGame
        {
            get { return TimeSpan.FromMilliseconds(M.ReadInt(Address + 0xc7c)); }
        }

    }
}