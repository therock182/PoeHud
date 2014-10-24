namespace PoeHUD.Poe.UI

{
    public class WindowState : Element
    {
        public bool IsVisibleLocal
        {
            get
            {
                return m.ReadInt(this.address + 0x860) == 1;
            }
        }
    }
}
