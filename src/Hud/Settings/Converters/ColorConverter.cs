using System;
using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using SharpDX;

namespace PoeHUD.Hud.Settings.Converters
{
    public class ColorConverter : CustomCreationConverter<Color>
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