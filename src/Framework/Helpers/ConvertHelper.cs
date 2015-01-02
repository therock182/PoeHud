namespace PoeHUD.Framework.Helpers
{
    public static class ConvertHelper
    {
        public static string ToShorten(double value, string format = "0")
        {
            if (value >= 1000000)
            {
                return string.Concat((value / 1000000).ToString(format), "M");
            }
            return value >= 1000 ? string.Concat((value / 1000).ToString(format), "K") : value.ToString(format);
        }
    }
}