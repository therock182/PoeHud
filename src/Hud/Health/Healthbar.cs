using PoeHUD.Controllers;
using PoeHUD.Models;

namespace PoeHUD.Hud.Health
{
	class Healthbar
	{
		public EntityWrapper entity;
		public string settings;
		public RenderPrio prio;
		public Healthbar(EntityWrapper entity, string settings, RenderPrio prio)
		{
			this.entity = entity;
			this.settings = settings;
			this.prio = prio;
		}
	}
}