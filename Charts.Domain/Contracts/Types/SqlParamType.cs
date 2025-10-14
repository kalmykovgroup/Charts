using System.Text.Json.Serialization;

namespace Charts.Api.Domain.Contracts.Types
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SqlParamType
    {
        // уже есть
        [JsonStringEnumMemberName("text")] Text = 0,
        [JsonStringEnumMemberName("string")] String = 0,

        [JsonStringEnumMemberName("int")] Int = 1,
        [JsonStringEnumMemberName("bigint")] Bigint = 2,

        [JsonStringEnumMemberName("double")] Double = 3,
        [JsonStringEnumMemberName("numeric")] Numeric = 3,  

        [JsonStringEnumMemberName("bool")] Bool = 4,
        [JsonStringEnumMemberName("uuid")] Uuid = 5,
        [JsonStringEnumMemberName("date")] Date = 6,
        [JsonStringEnumMemberName("timestamp")] Timestamp = 7,
        [JsonStringEnumMemberName("timestamptz")] Timestamptz = 8,

        // НОВОЕ (без смены существующих значений)
        [JsonStringEnumMemberName("real")] Real = 9,                // float4
        [JsonStringEnumMemberName("decimal")] Decimal = 10,         // точный decimal
        [JsonStringEnumMemberName("time")] Time = 11,
        [JsonStringEnumMemberName("timetz")] TimeTz = 12,
        [JsonStringEnumMemberName("interval")] Interval = 13,
        [JsonStringEnumMemberName("jsonb")] Jsonb = 14,
        [JsonStringEnumMemberName("json")] Json = 15,
        [JsonStringEnumMemberName("citext")] Citext = 16,
        [JsonStringEnumMemberName("bytea")] Bytea = 17,
        [JsonStringEnumMemberName("inet")] Inet = 18,
        [JsonStringEnumMemberName("cidr")] Cidr = 19,
        [JsonStringEnumMemberName("macaddr")] Macaddr = 20,

        // МАССИВЫ — явные варианты (проще, чем отдельный флаг)
        [JsonStringEnumMemberName("text[]")] TextArray = 100,
        [JsonStringEnumMemberName("int[]")] IntArray = 101,
        [JsonStringEnumMemberName("bigint[]")] BigintArray = 102,
        [JsonStringEnumMemberName("real[]")] RealArray = 103,
        [JsonStringEnumMemberName("double[]")] DoubleArray = 104,
        [JsonStringEnumMemberName("decimal[]")] DecimalArray = 105,
        [JsonStringEnumMemberName("uuid[]")] UuidArray = 106,
        [JsonStringEnumMemberName("date[]")] DateArray = 107,
        [JsonStringEnumMemberName("timestamp[]")] TimestampArray = 108,
        [JsonStringEnumMemberName("timestamptz[]")] TimestamptzArray = 109,
        [JsonStringEnumMemberName("jsonb[]")] JsonbArray = 110
    }

}
