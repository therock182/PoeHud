using System;
using System.Collections.Generic;

using SharpDX;

namespace PoeHUD.Hud
{
    public interface IPlugin : IDisposable
    {
        void Render(Dictionary<UiMountPoint, Vector2> mountPoints);
    }
}