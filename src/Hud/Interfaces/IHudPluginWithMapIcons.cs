using System.Collections.Generic;

namespace PoeHUD.Hud.Interfaces
{
    public interface IHudPluginWithMapIcons
    {
        IEnumerable<MapIcon> GetIcons();
    }
}