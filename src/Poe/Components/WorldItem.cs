namespace PoeHUD.Poe.Components
{
    public class WorldItem : Component
    {
        public Entity ItemEntity
        {
            get
            {
                if (Address != 0)
                {
                    return base.ReadObject<Entity>(Address + 20);
                }
                return base.GetObject<Entity>(0);
            }
        }
    }
}