using System;

using PoeHUD.Framework;
using PoeHUD.Models;
using PoeHUD.Poe.Components;

using SharpDX;

namespace PoeHUD.Hud
{
    public class MapIconCreature : MapIcon
    {
        public MapIconCreature(EntityWrapper entityWrapper, HudTexture hudTexture, Func<bool> show, int iconSize)
            : base(entityWrapper, hudTexture, show, iconSize) {}

        public override bool IsVisible()
        {
            return base.IsVisible() && EntityWrapper.IsAlive;
        }
    }

    public class MapIconChest : MapIcon
    {
        public MapIconChest(EntityWrapper entityWrapper, HudTexture hudTexture, Func<bool> show, int iconSize)
            : base(entityWrapper, hudTexture, show, iconSize) {}

        public override bool IsEntityStillValid()
        {
            return EntityWrapper.IsValid && !EntityWrapper.GetComponent<Chest>().IsOpened;
        }
    }

    public class MapIcon
    {
        private readonly Func<bool> show;

        public MapIcon(EntityWrapper entityWrapper, HudTexture hudTexture, Func<bool> show, int iconSize = 10)
        {
            EntityWrapper = entityWrapper;
            MinimapIcon = hudTexture;
            this.show = show;
            Size = iconSize;
        }

        public int? SizeOfLargeIcon { get; set; }

        public EntityWrapper EntityWrapper { get; set; }

        public HudTexture MinimapIcon { get; set; }

        public HudTexture LargeMapIcon { get; set; }

        public int Size { get; set; }

        public Vec2 WorldPosition
        {
            get { return EntityWrapper.GetComponent<Positioned>().GridPos; }
        }

        public static Vector2 deltaInWorldToMinimapDelta(Vec2 delta, double diag, float scale, float deltaZ = 0)
        {
            const double CameraAngle = Math.PI / 180 * 38;

            // Values according to 40 degree rotation of cartesian coordiantes, still doesn't seem right but closer
            var cosX = (float)(delta.X / scale * diag * Math.Cos(CameraAngle));
            var cosY = (float)(delta.Y / scale * diag * Math.Cos(CameraAngle));
            var sinX = (float)(delta.X / scale * diag * Math.Sin(CameraAngle));
            var sinY = (float)(delta.Y / scale * diag * Math.Sin(CameraAngle));
            // 2D rotation formulas not correct, but it's what appears to work?
            return new Vector2(cosX - cosY, -sinX - sinY + deltaZ);
        }

        public virtual bool IsEntityStillValid()
        {
            return EntityWrapper.IsValid;
        }

        public virtual bool IsVisible()
        {
            return show();
        }
    }
}