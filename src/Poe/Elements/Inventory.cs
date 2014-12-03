namespace PoeHUD.Poe.UI.Elements
{
    public class Inventory : Element
    {
        public RemoteMemoryObjects.Inventory InventoryModel
        {
            get { return base.ReadObject<RemoteMemoryObjects.Inventory>(Address + 2436); }
        }
    }
}