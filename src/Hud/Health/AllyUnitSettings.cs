using PoeHUD.Hud.Settings;

using SharpDX;

namespace PoeHUD.Hud.Health
{
    public class AllyUnitSettings : SettingsBase
    {
        public AllyUnitSettings() {}

        public AllyUnitSettings(uint color, uint outline)
        {
            Enable = true;
            Width = new RangeNode<float>(105, 50, 180);
            Height = new RangeNode<float>(25, 10, 50);
            Color = Color.FromAbgr(color);
            Outline = Color.FromAbgr(outline);
        }

        public RangeNode<float> Width { get; set; }

        public RangeNode<float> Height { get; set; }

        public Color Color { get; set; }

        public Color Outline { get; set; }
    }
}