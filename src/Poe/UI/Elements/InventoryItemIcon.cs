namespace PoeHUD.Poe.UI.Elements
{
    public class InventoryItemIcon : Element
    {
        public Tooltip Tooltip
        {
            get { return base.ReadObject<Tooltip>(Address + 2796); }
        }

        public Entity Item
        {
            get { return base.ReadObject<Entity>(Address + 2832); }
        }
    }
}