using System;
using PoeHUD.Models;
using PoeHUD.Poe;
using PoeHUD.Poe.RemoteMemoryObjects;

namespace PoeHUD.Controllers
{
	public class AreaController
	{

		private readonly GameController Root;

		public AreaController(GameController gameController)
		{
			Root = gameController;
		}

		public event Action<AreaController> OnAreaChange;

		public AreaInstance CurrentArea { get; private set; }

		// dict is wrong 'cause hash is wrong
		// public Dictionary<int, AreaInstance> AreasVisited = new Dictionary<int, AreaInstance>();

		public void RefreshState()
		{
			var igsd = this.Root.Game.IngameState.Data;
			AreaTemplate clientsArea = igsd.CurrentArea;
			int curAreaHash = igsd.CurrentAreaHash;

			if (CurrentArea != null && curAreaHash == CurrentArea.Hash)
				return;

			// try to find the new area in our dictionary
			AreaInstance //area;
			//if (!AreasVisited.TryGetValue(curAreaHash, out area)) {
				area = new AreaInstance(clientsArea, curAreaHash, igsd.CurrentAreaLevel);
			// }

			CurrentArea = area;

			this.OnAreaChange(this);
		}
	}
}
