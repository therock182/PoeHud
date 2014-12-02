namespace PoeHUD.Poe.Components
{
    public class Quality : Component
    {
        public int ItemQuality
        {
            get
            {
                if (Address != 0)
                {
                    return M.ReadInt(Address + 12);
                }
                return 0;
            }
        }
    }
}