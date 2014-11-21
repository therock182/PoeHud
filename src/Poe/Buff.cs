namespace PoeHUD.Poe
{
    public class Buff : RemoteMemoryObject
    {
        public string Name
        {
            get { return M.ReadStringU(M.ReadInt(Address + 4, 0)); }
        }

        public int Charges
        {
            get { return M.ReadInt(Address + 24); }
        }

        public int SkillId
        {
            get { return M.ReadInt(Address + 36); }
        }

        public float Timer
        {
            get { return M.ReadFloat(Address + 12); }
        }
    }
}