using PoeHUD.Framework;

namespace PoeHUD.Poe
{
    public sealed class TheGame : RemoteMemoryObject
    {
        public TheGame(Memory m)
        {
            M = m;
            Address = m.ReadInt(m.AddressOfProcess + Offsets.Base, new[] {4, 124});
            Game = this;
        }

        public IngameState IngameState
        {
            get { return base.ReadObject<IngameState>(Address + 0x9C); }
        }

        public int AreaChangeCount
        {
            get { return M.ReadInt(M.AddressOfProcess + Offsets.AreaChangeCount); }
        }
    }
}