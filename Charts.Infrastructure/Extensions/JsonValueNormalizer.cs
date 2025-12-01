using System.Text.Json;

namespace Charts.Infrastructure.Extensions;

public static class JsonValueNormalizer
{
    public static object? ToClr(object? value)
    {
        return value switch
        {
            JsonElement el => FromJsonElement(el),
            JsonDocument doc => FromJsonElement(doc.RootElement),
            _ => value
        };
    }
     
    private static object? FromJsonElement(JsonElement el)
    {
        switch (el.ValueKind)
        {
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            case JsonValueKind.True: return true;
            case JsonValueKind.False: return false;
            case JsonValueKind.String:
                // если строка — это дата/время, можно попытаться распарсить:
                if (el.TryGetDateTime(out var dt)) return dt;
                return el.GetString();
            case JsonValueKind.Number:
                if (el.TryGetInt64(out var l)) return l;
                if (el.TryGetDouble(out var d)) return d;
                // запасной вариант — decimal
                return el.GetDecimal();
            case JsonValueKind.Array:
                {
                    var arr = new List<object?>();
                    foreach (var item in el.EnumerateArray())
                        arr.Add(FromJsonElement(item));
                    return arr.ToArray();
                }
            case JsonValueKind.Object:
                {
                    var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                    foreach (var prop in el.EnumerateObject())
                        dict[prop.Name] = FromJsonElement(prop.Value);
                    return dict;
                }
            default:
                return default;
        }
    }
}


