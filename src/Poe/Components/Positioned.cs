using PoeHUD.Framework;

using SharpDX;

namespace PoeHUD.Poe.Components
{
    public class Positioned : Component
    {
        public float X
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadFloat(Address + 0x1C);
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
                    return M.ReadFloat(Address + 0x20);
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
                    return M.ReadInt(Address + 0x10);
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
                    return M.ReadInt(Address + 0x14);
                }
                return 0;
            }
        }

        public Vector2 GridPos
        {
            get { return new Vector2(GridX, GridY); }
        }
    }
}