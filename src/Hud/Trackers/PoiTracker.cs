using System.Collections.Generic;

using PoeHUD.Controllers;
using PoeHUD.Hud.Interfaces;
using PoeHUD.Hud.UI;
using PoeHUD.Models;
using PoeHUD.Poe.Components;

using SharpDX;

namespace PoeHUD.Hud.Trackers
{
	public class PoiTracker : Plugin<PoiTrackerSettings>, IPluginWithMapIcons
	{
		private readonly Dictionary<EntityWrapper, MapIcon> currentIcons = new Dictionary<EntityWrapper, MapIcon>();


	    public PoiTracker(GameController gameController, Graphics graphics, PoiTrackerSettings settings)
            : base(gameController, graphics, settings)
	    {
            this.GameController.Area.OnAreaChange += this.CurrentArea_OnAreaChange;

            currentIcons.Clear();
            foreach (EntityWrapper current in this.GameController.Entities)
            {
                this.OnEntityAdded(current);
            }
	    }

	
		protected override void OnEntityRemoved(EntityWrapper entity)
		{
			currentIcons.Remove(entity);
		}

        protected override void OnEntityAdded(EntityWrapper entity)
		{
			if (!Settings.Enable)
			{
				return;
			}
			var icon = GetMapIcon(entity);
			if ( null != icon )
				currentIcons[entity] = icon;

		}
		private void CurrentArea_OnAreaChange(AreaController area)
		{
			currentIcons.Clear();
		}
		public override void Render(Dictionary<UiMountPoint, Vector2> mountPoints)
		{
            if (!Settings.Enable)
			{
				return;
			}
		}

		public IEnumerable<MapIcon> GetIcons()
		{
			List<EntityWrapper> toRemove = new List<EntityWrapper>();
			foreach (KeyValuePair<EntityWrapper, MapIcon> kv in currentIcons)
			{
				if (kv.Value.IsEntityStillValid())
					yield return kv.Value;
				else
					toRemove.Add(kv.Key);
			}
			foreach (EntityWrapper wrapper in toRemove)
			{
				currentIcons.Remove(wrapper);
			}
		}


		private static readonly List<string> masters = new List<string> {
			"Metadata/NPC/Missions/Wild/Dex",
			"Metadata/NPC/Missions/Wild/DexInt",
			"Metadata/NPC/Missions/Wild/Int",
			"Metadata/NPC/Missions/Wild/Str",
			"Metadata/NPC/Missions/Wild/StrDex",
			"Metadata/NPC/Missions/Wild/StrDexInt",
			"Metadata/NPC/Missions/Wild/StrInt"
		};

		private MapIcon GetMapIcon(EntityWrapper e)
		{
			if (e.HasComponent<NPC>() && masters.Contains(e.Path))
			{
				return new MapIconCreature(e, new HudTexture("monster_ally.png"), () => Settings.Masters, 10);
			}
			if (e.HasComponent<Chest>() && !e.GetComponent<Chest>().IsOpened)
			{
			    return e.GetComponent<Chest>().IsStrongbox
			        ? new MapIconChest(e, new HudTexture("strongbox.png", e.GetComponent<ObjectMagicProperties>().Rarity), () => Settings.Strongboxes, 16)
			        : new MapIconChest(e, new HudTexture("minimap_default_icon.png"), () => Settings.Chests, 6);
			}
		    return null;

		}

	
	}
}
