using System.Collections.Generic;

namespace PoeHUD.Hud.Interfaces
{
    public interface IHudPluginWithMapIcons : IHudPlugin
    {
        IEnumerable<MapIcon> GetIcons();
    }
}