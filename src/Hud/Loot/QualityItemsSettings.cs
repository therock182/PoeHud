using PoeHUD.Hud.Settings;

namespace PoeHUD.Hud.Loot
{
    public class QualityItemsSettings : SettingsBase
    {
        public QualityItemsSettings()
        {
            Enable = false;
            Weapon = new QualityItemSettings(12);
            Armour = new QualityItemSettings(12);
            Flask = new QualityItemSettings(10);
        }

        public QualityItemSettings Weapon { get; set; }

        public QualityItemSettings Armour { get; set; }

        public QualityItemSettings Flask { get; set; }
    }
}