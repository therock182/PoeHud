namespace PoeHUD.Poe
{
    public class ServerData : RemoteMemoryObject
    {
        public bool IsInGame
        {
            get { return M.ReadInt(Address + 11000) == 3; //closer to league name
            }
        }

        public InventorySet PlayerInventories
        {
            get { return base.GetObject<InventorySet>(Address + 10496); }
        }
    }
}