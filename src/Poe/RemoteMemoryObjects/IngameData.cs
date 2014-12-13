namespace PoeHUD.Poe.RemoteMemoryObjects
{
    public class IngameData : RemoteMemoryObject
    {
        public AreaTemplate CurrentArea
        {
            get { return base.ReadObject<AreaTemplate>(Address + 8); }
        }

        public int CurrentAreaLevel
        {
            get { return M.ReadInt(Address + 12); }
        }

        public int CurrentAreaHash
        {
            get { return M.ReadInt(Address + 16); }
        }

        public Entity LocalPlayer
        {
            get { return base.ReadObject<Entity>(Address + 1440); }
        }

        public EntityList EntityList
        {
            get
            {
                return base.GetObject<EntityList>(Address + 1476);
            }
        }
    }
}