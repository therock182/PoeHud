namespace PoeHUD.Poe.Components
{
    public class Player : Component
    {
        public string PlayerName
        {
            get
            {
                if (Address == 0)
                {
                    return "";
                }
                int num = M.ReadInt(Address + 32);
                if (num > 512)
                {
                    return "";
                }
                if (num < 8)
                {
                    return M.ReadStringU(Address + 16, num*2);
                }
                return M.ReadStringU(M.ReadInt(Address + 16), num*2);
            }
        }

        public long XP
        {
            get
            {
                if (Address != 0)
                {
                    return (long) M.ReadUInt(Address + 52);
                }
                return 0L;
            }
        }

        public int Level
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 68);
                }
                return 1;
            }
        }
    }
}