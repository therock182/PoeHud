using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.Interfaces;
using PoeHUD.Hud.UI;
using PoeHUD.Poe.Components;

using SharpDX;
using SharpDX.Direct3D9;

namespace PoeHUD.Hud.DPS
{
	public class DpsMeter : Plugin
	{

        
		private bool hasStarted;
		private DateTime lastCalcTime;
		private Dictionary<int, int> lastEntities;
	    
		
		private const float dps_period = 0.2f;
		private readonly float[] damageMemory = new float[10];
		private int ixDamageMemory;
		private int maxDps;


	    public DpsMeter(GameController gameController, Graphics graphics) : base(gameController, graphics)
	    {
            lastEntities = new Dictionary<int, int>();
            GameController.Area.OnAreaChange += CurrentArea_OnAreaChange;
	    }

	
		private void CurrentArea_OnAreaChange(AreaController area)
		{
			lastEntities = new Dictionary<int, int>();
			hasStarted = false;
			maxDps = 0;
		}

		public override void Render(Dictionary<UiMountPoint, Vector2> mountPoints)
		{
			if (!Settings.GetBool("DpsDisplay"))
			{
				return;
			}

			if (!hasStarted)
			{
				lastCalcTime = DateTime.Now;
				hasStarted = true;
				return;
			}

			DateTime dtNow = DateTime.Now;
			TimeSpan delta = dtNow - lastCalcTime;

			if (delta.TotalSeconds > dps_period)
			{
				ixDamageMemory++;
				if (ixDamageMemory >= damageMemory.Length)
					ixDamageMemory = 0;
				damageMemory[ixDamageMemory] = CalculateDps(delta);
				lastCalcTime = dtNow;
			}

			int fontSize = Settings.GetInt("XphDisplay.FontSize");
			Vector2 mapWithOffset = mountPoints[UiMountPoint.LeftOfMinimap];
			int dps = ((int)damageMemory.Average());
			if (maxDps < dps)
				maxDps = dps;

            var textSize = Graphics.DrawText(dps + " DPS", fontSize * 3f / 2f, mapWithOffset, Color.White, FontDrawFlags.Right);
            var tx2 = Graphics.DrawText(maxDps + " peak DPS", fontSize * 2f / 3f, new Vector2(mapWithOffset.X, mapWithOffset.Y + textSize.Height), Color.White, FontDrawFlags.Right);

			int width = Math.Max(tx2.Width, textSize.Width);
			var rect = new RectangleF(mapWithOffset.X - 5 - width, mapWithOffset.Y - 5, width + 10, textSize.Height + tx2.Height + 10);

		    Color backgroundColor = Color.Black;
		    backgroundColor.A = 160;
            Graphics.DrawBox(rect, backgroundColor);

			mountPoints[UiMountPoint.LeftOfMinimap] = new Vector2(mapWithOffset.X, mapWithOffset.Y + 5 + rect.Height);
		}

		private float CalculateDps(TimeSpan dt)
		{
			var currentEntities = new Dictionary<int, int>();
            int damageDoneThisCycle = 0;

			foreach (var entity in GameController.Entities.Where(x => x.HasComponent<Poe.Components.Monster>() && x.IsHostile))
			{
				int entityEHP = entity.IsAlive ? entity.GetComponent<Life>().CurHP + entity.GetComponent<Life>().CurES : 0;
				if (entityEHP > 10000000 || entityEHP < -1000000) //discard those - read form invalid addresses
					continue;

				if (lastEntities.ContainsKey(entity.Id))
				{
					if (lastEntities[entity.Id] > entityEHP) damageDoneThisCycle += lastEntities[entity.Id] - entityEHP;
				}

				currentEntities.Add(entity.Id, entityEHP);
			}
			// cache current life/es values for next check
			lastEntities = currentEntities;

			return (float)(damageDoneThisCycle / dt.TotalSeconds);
		}

	}
}
