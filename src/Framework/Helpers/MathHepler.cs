using System;
using System.Linq;

using SharpDX;

namespace PoeHUD.Framework.Helpers
{
    public static class MathHepler
    {
        public static float Max(params float[] values)
        {
            float max = values.First();
            for (int i = 1; i < values.Length; i++)
            {
                max = Math.Max(max, values[i]);
            }
            return max;
        }

        public static Vector2 Translate(this Vector2 vector, float dx, float dy)
        {
            return new Vector2(vector.X + dx, vector.Y + dy);
        }
    }
}