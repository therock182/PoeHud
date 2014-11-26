using System.Collections.Generic;

namespace PoeHUD.Controllers
{
    public class EntityListObserverComposite : IEntityListObserver
    {
        public readonly List<IEntityListObserver> Observers = new List<IEntityListObserver>();
        public void EntityRemoved(EntityWrapper entity)
        {
            foreach (var observer in Observers)
                observer.EntityRemoved(entity);
        }

        public void EntityAdded(EntityWrapper entity)
        {
            foreach (var observer in Observers)
                observer.EntityAdded(entity);
        }
    }
}