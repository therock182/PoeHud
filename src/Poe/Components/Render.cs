using PoeHUD.Framework;

namespace PoeHUD.Poe.Components
{
    public class Render : Component
    {
        public float X
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadFloat(Address + 48);
                }
                return 0f;
            }
        }

        public float Y
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadFloat(Address + 52);
                }
                return 0f;
            }
        }

        public float Z
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadFloat(Address + 136);
                }
                return 0f;
            }
        }

        public Vec3 Pos
        {
            get { return new Vec3(X, Y, Z); }
        }

        public string DisplayName
        {
            get
            {
                if (Address == 0)
                {
                    return "";
                }
                int num = M.ReadInt(Address + 88);
                if (num < 8)
                {
                    return M.ReadStringU(Address + 72, 16);
                }
                return M.ReadStringU(M.ReadInt(Address + 72));
            }
        }
    }
}