using PoeHUD.Framework;

namespace PoeHUD.Poe.EntityComponents
{
    public class Positioned : Component
    {
        public float X
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadFloat(Address + 44);
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
                    return M.ReadFloat(Address + 48);
                }
                return 0f;
            }
        }

        public int GridX
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 52);
                }
                return 0;
            }
        }

        public int GridY
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 56);
                }
                return 0;
            }
        }

        public Vec2 GridPos
        {
            get { return new Vec2(GridX, GridY); }
        }
    }
}