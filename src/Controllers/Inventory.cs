using System.Collections.Generic;

namespace PoeHUD.Controllers
{
	public class Inventory
	{
		private Poe.Inventory InternalInventory;
		private GameController Poe;
		public int Width
		{
			get
			{
				return this.InternalInventory.Width;
			}
		}
		public int Height
		{
			get
			{
				return this.InternalInventory.Height;
			}
		}
		public List<EntityWrapper> Items
		{
			get
			{
				List<EntityWrapper> list = new List<EntityWrapper>();
				foreach (Poe.Entity current in this.InternalInventory.Items)
				{
					list.Add(new EntityWrapper(this.Poe, current));
				}
				return list;
			}
		}
		public Inventory(GameController poe, Poe.Inventory internalInventory)
		{
			Poe = poe;
			InternalInventory = internalInventory;
		}
		public Inventory(GameController poe, int address) : this(poe, poe.Game.GetObject<Poe.Inventory>(address))
		{
		}
	}
}
