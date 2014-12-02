namespace PoeHUD.Poe
{
    public abstract class Component : RemoteMemoryObject
    {
        protected Entity Owner
        {
            get { return base.ReadObject<Entity>(Address + 4); }
        }
    }
}