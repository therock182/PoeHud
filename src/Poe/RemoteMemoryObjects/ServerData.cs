namespace PoeHUD.Poe.RemoteMemoryObjects
{
    public class ServerData : RemoteMemoryObject
    {
        public bool IsInGame
        {
            get { return M.ReadInt(Address + 11000) == 3; //closer to league name
            }
        }

        public InventoryList PlayerInventories
        {
            get { return base.GetObject<InventoryList>(Address + 10496); }
        }
    }
}