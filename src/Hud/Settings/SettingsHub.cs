using System;
using System.Globalization;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using PoeHUD.Hud.DPS;
using PoeHUD.Hud.Health;
using PoeHUD.Hud.Icons;
using PoeHUD.Hud.Loot;
using PoeHUD.Hud.MaxRolls;
using PoeHUD.Hud.Menu;
using PoeHUD.Hud.MiscHacks;
using PoeHUD.Hud.Monster;
using PoeHUD.Hud.Preload;
using PoeHUD.Hud.XpRate;

using SharpDX;

namespace PoeHUD.Hud.Settings
{
    public sealed class SettingsHub
    {
        private const string SETTINGS_FILE_NAME = "config/settings.json";

        public SettingsHub()
        {
            WindowName = "Something in Galaxy";
            MenuSettings = new MenuSettings();
            DpsMeterSettings = new DpsMeterSettings();
            LargeMapSettings = new LargeMapSettings();
            MinimapSettings = new MinimapSettings();
            ItemAlertSettings = new ItemAlertSettings();
            ItemLevelSettings = new ItemLevelSettings();
            ItemModsSettings = new ItemModsSettings();
            MonsterTrackerSettings = new MonsterTrackerSettings();
            PoiTrackerSettings = new PoiTrackerSettings();
            PreloadAlertSettings = new PreloadAlertSettings();
            XpRateSettings = new XpRateSettings();
            MiscHacksSettings = new MiscHacksSettings();
            HealthBarSettings = new HealthBarSettings();
        }

        public string WindowName { get; set; }

        [JsonProperty("Menu")]
        public MenuSettings MenuSettings { get; set; }

        [JsonProperty("DPS meter")]
        public DpsMeterSettings DpsMeterSettings { get; set; }

        [JsonProperty("Icons on large map")]
        public LargeMapSettings LargeMapSettings { get; set; }

        [JsonProperty("Icons on minimap")]
        public MinimapSettings MinimapSettings { get; set; }

        [JsonProperty("Item alert")]
        public ItemAlertSettings ItemAlertSettings { get; set; }

        [JsonProperty("Item level")]
        public ItemLevelSettings ItemLevelSettings { get; set; }

        [JsonProperty("Item mods")]
        public ItemModsSettings ItemModsSettings { get; set; }

        [JsonProperty("Monster tracker")]
        public MonsterTrackerSettings MonsterTrackerSettings { get; set; }

        [JsonProperty("Poi tracker")]
        public PoiTrackerSettings PoiTrackerSettings { get; set; }

        [JsonProperty("Preload alert")]
        public PreloadAlertSettings PreloadAlertSettings { get; set; }

        [JsonProperty("XP per hour")]
        public XpRateSettings XpRateSettings { get; set; }

        [JsonProperty("Misc hacks")]
        public MiscHacksSettings MiscHacksSettings { get; set; }

        [JsonProperty("Health bar")]
        public HealthBarSettings HealthBarSettings { get; set; }

        public static SettingsHub Load()
        {
            try
            {
                string json = File.ReadAllText(SETTINGS_FILE_NAME);
                return JsonConvert.DeserializeObject<SettingsHub>(json, new ColorConverter());
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
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented, new ColorConverter());
                stream.Write(json);
            }
        }

        private class ColorConverter : CustomCreationConverter<Color>
        {
            public override bool CanWrite
            {
                get { return true; }
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override Color Create(Type objectType)
            {
                return new Color();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                int argb;
                return int.TryParse(reader.Value.ToString(), NumberStyles.HexNumber, null, out argb)
                    ? Color.FromAbgr(argb)
                    : Create(objectType);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var color = (Color)value;
                serializer.Serialize(writer, string.Format("{0:x8}", color.ToAbgr()));
            }
        }
    }
}