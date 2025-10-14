using System.Text.Json;
using System.Text.Json.Serialization;

namespace Charts.Api.Halpers
{
    public class DateTimeOffsetUtcJsonConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dto = DateTimeOffset.Parse(reader.GetString()!);
            return dto.ToUniversalTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("O"));
        }
    }
}
