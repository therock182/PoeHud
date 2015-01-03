using System;
using System.IO;

using Newtonsoft.Json;

using PoeHUD.Hud.Dps;
using PoeHUD.Hud.Health;
using PoeHUD.Hud.Icons;
using PoeHUD.Hud.Loot;
using PoeHUD.Hud.MaxRolls;
using PoeHUD.Hud.Menu;
using PoeHUD.Hud.MiscHacks;
using PoeHUD.Hud.Preload;
using PoeHUD.Hud.Settings.Converters;
using PoeHUD.Hud.Trackers;
using PoeHUD.Hud.XpRate;

namespace PoeHUD.Hud.Settings
{
    public sealed class SettingsHub
    {
        private const string SETTINGS_FILE_NAME = "config/settings.json";

        private static readonly JsonSerializerSettings jsonSettings;

        static SettingsHub()
        {
            jsonSettings = new JsonSerializerSettings();
            jsonSettings.ContractResolver = new SortContractResolver();
            jsonSettings.Converters = new JsonConverter[]
            {
                new ColorConverter(),
                new ToggleNodeConverter()
            };
        }

        public SettingsHub()
        {
            WindowName = "Something in Galaxy";
            MenuSettings = new MenuSettings();
            DpsMeterSettings = new DpsMeterSettings();
            LargeMapSettings = new LargeMapSettings();
            MinimapSettings = new MinimapSettings();
            ItemAlertSettings = new ItemAlertSettings();
            ItemLevelSettings = new ItemLevelSettings();
            ItemRollsSettings = new ItemRollsSettings();
            MonsterTrackerSettings = new MonsterTrackerSettings();
            PoiTrackerSettings = new PoiTrackerSettings();
            PreloadAlertSettings = new PreloadAlertSettings();
            XpRateSettings = new XpRateSettings();
            MiscHacksSettings = new MiscHacksSettings();
            HealthBarSettings = new HealthBarSettings();
        }

        public string WindowName { get; private set; }

        [JsonProperty("Menu")]
        public MenuSettings MenuSettings { get; private set; }

        [JsonProperty("DPS meter")]
        public DpsMeterSettings DpsMeterSettings { get; private set; }

        [JsonProperty("Icons on large map")]
        public LargeMapSettings LargeMapSettings { get; private set; }

        [JsonProperty("Icons on minimap")]
        public MinimapSettings MinimapSettings { get; private set; }

        [JsonProperty("Item alert")]
        public ItemAlertSettings ItemAlertSettings { get; private set; }

        [JsonProperty("Item level")]
        public ItemLevelSettings ItemLevelSettings { get; private set; }

        [JsonProperty("Item rolls")]
        public ItemRollsSettings ItemRollsSettings { get; private set; }

        [JsonProperty("Monster tracker")]
        public MonsterTrackerSettings MonsterTrackerSettings { get; private set; }

        [JsonProperty("Poi tracker")]
        public PoiTrackerSettings PoiTrackerSettings { get; private set; }

        [JsonProperty("Preload alert")]
        public PreloadAlertSettings PreloadAlertSettings { get; private set; }

        [JsonProperty("XP per hour")]
        public XpRateSettings XpRateSettings { get; private set; }

        [JsonProperty("Misc hacks")]
        public MiscHacksSettings MiscHacksSettings { get; private set; }

        [JsonProperty("Health bar")]
        public HealthBarSettings HealthBarSettings { get; private set; }

        public static SettingsHub Load()
        {
            try
            {
                string json = File.ReadAllText(SETTINGS_FILE_NAME);
                return JsonConvert.DeserializeObject<SettingsHub>(json, jsonSettings);
            }
            catch
            {
                if (File.Exists(SETTINGS_FILE_NAME))
                {
                    string backupFileName = SETTINGS_FILE_NAME + DateTime.Now.Ticks;
                    File.Move(SETTINGS_FILE_NAME, backupFileName);
                }

                var settings = new SettingsHub();
                Save(settings);
                return settings;
            }
        }

        public static void Save(SettingsHub settings)
        {
            using (var stream = new StreamWriter(File.Create(SETTINGS_FILE_NAME)))
            {
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented, jsonSettings);
                stream.Write(json);
            }
        }
    }
}