namespace PoeHUD.Poe.EntityComponents
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