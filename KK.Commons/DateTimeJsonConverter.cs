using System.Text.Json;
using System.Text.Json.Serialization;

namespace KK.Commons
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        private string format;

        public DateTimeJsonConverter()
        {
            format = "yyyy-MM-dd HH:mm:ss";
        }

        public DateTimeJsonConverter(string format)
        {
            this.format = format;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? str = reader.GetString();
            if (str == null)
            {
                return default(DateTime);
            }
            return DateTime.Parse(str);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(format));
        }
    }
}
