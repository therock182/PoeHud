using SharpDX;

namespace PoeHUD.Models
{
    public class PreloadAlerConfigLine
    {
        public string Text { get; set; }
        public Color? Color { get; set; }

        public override bool Equals(object obj)
        {
            return Text == ((PreloadAlerConfigLine) obj).Text;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }
    }
}