using System;
using System.Collections.Generic;
using PoeHUD.Controllers;
using PoeHUD.Framework;
using PoeHUD.Hud.Interfaces;
using PoeHUD.Models;

namespace PoeHUD.Hud
{
    public abstract class HudPluginBase : IDisposable
    {
        protected readonly GameController GameController;

        protected HudPluginBase( GameController gameController)
        {
            GameController = gameController;
            gameController.EntityListWrapper.OnEntityAdded += OnEntityAdded;
            gameController.EntityListWrapper.OnEntityRemoved += OnEntityRemoved;
        }
        
        public virtual void OnEntityAdded(EntityWrapper entity)
        {

        }

        public virtual void OnEntityRemoved(EntityWrapper entity)
        {

        }

        public abstract void Render(RenderingContext rc, Dictionary<UiMountPoint, Vec2> mountPoints);

        // could not find a better place yet
        protected static RectUV GetDirectionsUv(double phi, double distance)
        {
            phi += Math.PI * 0.25; // fix rotation due to projection
            if (phi > 2 * Math.PI)
                phi -= 2 * Math.PI;
            float xSprite = (float)Math.Round(phi / Math.PI * 4);
            if (xSprite >= 8) xSprite = 0;
            float ySprite = distance > 60 ? distance > 120 ? 2 : 1 : 0;
            var rectUV = new RectUV(xSprite / 8, ySprite / 3, (xSprite + 1) / 8, (ySprite + 1) / 3);
            return rectUV;
        }

        public virtual void Dispose()
        {
            
        }
    }
}