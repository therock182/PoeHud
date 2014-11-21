namespace PoeHUD.Poe.EntityComponents
{
    public class Weapon : Component
    {
        public int DamageMin
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 16, 4);
                }
                return 0;
            }
        }

        public int DamageMax
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 16, 8);
                }
                return 0;
            }
        }

        public int AttackTime
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 16, 12);
                }
                return 1;
            }
        }

        public int CritChance
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 16, 16);
                }
                return 0;
            }
        }
    }
}