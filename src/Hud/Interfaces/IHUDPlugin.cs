using System.Collections.Generic;
using PoeHUD.Controllers;
using PoeHUD.Framework;

namespace PoeHUD.Hud.Interfaces
{
	public interface IHudPlugin
	{
		void Init(GameController poe);
		void OnEnable();
		void OnDisable();

		void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints);
	}
}
