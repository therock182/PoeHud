namespace PoeHUD.Poe.EntityComponents
{
    public class Stack : Component
    {
        public int Size
        {
            get
            {
                if (Address == 0)
                {
                    return 0;
                }
                int res = M.ReadInt(Address + 12);
                return res;
            }
        }
    }
}