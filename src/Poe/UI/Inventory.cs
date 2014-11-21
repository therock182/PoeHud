namespace PoeHUD.Poe.UI
{
    public class Inventory : Element
    {
        public Poe.Inventory InventoryModel
        {
            get { return base.ReadObject<Poe.Inventory>(Address + 2436); }
        }
    }
}