using Newtonsoft.Json;

using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.Loot
{
    public sealed class ItemAlertSettings : SettingsBase
    {
        public ItemAlertSettings()
        {
            Enable = true;
            Crafting = true;
            ShowText = true;
            PlaySound = true;
            TextSize = new RangeNode<int>(25, 10, 50);
            Rares = true;
            Uniques = true;
            Maps = true;
            Currency = true;
            SkillGems = true;
            QualitySkillGems = true;
            Rgb = true;
            MinLinks = new RangeNode<int>(5, 0, 6);
            MinSockets = new RangeNode<int>(6, 0, 6);
            QualitySkillGemsLevel = new RangeNode<int>(1, 1, 30);
        }

        public ToggleNode Crafting { get; set; }

        public ToggleNode ShowText { get; set; }

        public ToggleNode PlaySound { get; set; }

        public RangeNode<int> TextSize { get; set; }

        public ToggleNode Rares { get; set; }

        public ToggleNode Uniques { get; set; }

        public ToggleNode Maps { get; set; }

        public ToggleNode Currency { get; set; }

        public ToggleNode SkillGems { get; set; }

        public ToggleNode QualitySkillGems { get; set; }

        [JsonProperty("RGB")]
        public ToggleNode Rgb { get; set; }

        public RangeNode<int> MinLinks { get; set; }

        public RangeNode<int> MinSockets { get; set; }

        public RangeNode<int> QualitySkillGemsLevel { get; set; }
    }
}