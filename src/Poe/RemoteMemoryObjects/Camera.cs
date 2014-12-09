using System;
using System.Threading;
using PoeHUD.Framework;
using PoeHUD.Models;
using PoeHUD.Poe.Components;
using SlimDX;

namespace PoeHUD.Poe.RemoteMemoryObjects
{
    public class Camera : RemoteMemoryObject
    {
        public int Width
        {
            get { return M.ReadInt(Address + 4); }
        }

        public int Height
        {
            get { return M.ReadInt(Address + 8); }
        }

        public float ZFar
        {
            get { return M.ReadFloat(Address + 392); }
            set { M.WriteFloat(Address + 392, value); }
        }

        public Vec3 Position
        {
            get { return new Vec3(M.ReadFloat(Address + 256), M.ReadFloat(Address + 260), M.ReadFloat(Address + 264)); }
        }

        static Vector4 oldtranslation;
        static Vec2 oldplayerCord;
        public unsafe Vec2 WorldToScreen(Vec3 vec3, EntityWrapper entityWrapper, int times = 0)
        {

            var isplayer = Game.IngameState.Data.LocalPlayer.IsValid && Game.IngameState.Data.LocalPlayer.Address == entityWrapper.Address;
            var isMoving = Game.IngameState.Data.LocalPlayer.GetComponent<Actor>().isMoving;
            float x, y, z;
            int addr = base.Address + 0xbc;
            fixed (byte* numRef = base.M.ReadBytes(addr, 0x40))
            {
                Matrix matrix = *(Matrix*)numRef;
                var translation = *(Vector4*)&matrix.M41;
                Vector4 cord = *(Vector4*)&vec3;
                cord.W = 1;
                cord = Vector4.Transform(cord, matrix);
                cord = Vector4.Divide(cord, cord.W);
                x = ((cord.X + 1.0f) * 0.5f) * Width;
                y = ((1.0f - cord.Y) * 0.5f) * Height;

                if (times < 50000 && isMoving && isplayer && oldtranslation == translation)
                {
                    return WorldToScreen(vec3, entityWrapper, times + 1);
                }
                oldtranslation = translation;
            }
            var resultCord = new Vec2((int)Math.Round(x), (int)Math.Round(y));
            if (isMoving && isplayer)
            {
                if (Math.Abs(oldplayerCord.X - resultCord.X) < 40 || (Math.Abs(oldplayerCord.X - resultCord.Y) < 40))
                    resultCord = oldplayerCord;
                else
                    oldplayerCord = resultCord;
            }
            else if (isplayer)
            {
                oldplayerCord = resultCord;
            }
            return resultCord;
        }
    }
}