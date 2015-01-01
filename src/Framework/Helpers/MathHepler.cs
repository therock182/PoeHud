using SharpDX;

namespace PoeHUD.Framework.Helpers
{
    public static class MathHepler
    {
        public static Vector2 Translate(this Vector2 vector, float dx, float dy)
        {
            return new Vector2(vector.X + dx, vector.Y + dy);
        }
    }
}