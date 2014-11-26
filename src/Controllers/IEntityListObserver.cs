namespace PoeHUD.Controllers
{
    public interface IEntityListObserver {
        void EntityAdded(EntityWrapper entity);
        void EntityRemoved(EntityWrapper entity);
    }
}