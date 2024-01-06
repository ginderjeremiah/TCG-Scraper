using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace CommonLibrary
{
    public static class Extensions
    {
        private static readonly JsonSerializerOptions _options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        public static string AsString(this object? obj, string? defaultVal = null)
        {
            return obj switch
            {
                IEnumerable<object?> enumerable => string.Join(",", enumerable.Select(o => o.AsString())),
                _ => obj?.ToString() ?? defaultVal ?? string.Empty,
            };
        }

        public static int AsInt(this object? obj, int defaultVal = 0)
        {
            if (obj is int intObj)
                return intObj;
            else if (int.TryParse(obj.AsString(), out var val))
                return val;
            else
                return defaultVal;
        }

        public static DateTime AsDate(this object? obj, DateTime? defaultVal = null)
        {
            if (obj is DateTime dtObj)
                return dtObj;
            else if (DateTime.TryParse(obj.AsString(), out var dt))
                return dt;
            else
                return defaultVal ?? DateTime.MinValue;
        }

        public static bool AsBool(this object? obj, bool defaultVal = false)
        {
            if (obj is bool boolObj)
                return boolObj;
            else if (obj is int intObj)
                return !(intObj == 0);
            else if (bool.TryParse(obj.AsString(), out var b))
                return b;
            else
                return defaultVal;
        }

        public static short AsShort(this object? obj, short defaultVal = 0)
        {
            if (obj is short intObj)
                return intObj;
            else if (short.TryParse(obj.AsString(), out var val))
                return val;
            else
                return defaultVal;
        }

        public static float AsFloat(this object? obj, float defaultVal = 0f)
        {
            if (obj is float intObj)
                return intObj;
            else if (float.TryParse(obj.AsString(), out var val))
                return val;
            else
                return defaultVal;
        }

        public static T? Deserialize<T>(this HttpResponseMessage msg)
        {
            return JsonSerializer.Deserialize<T>(msg.Content.ReadAsStream(), _options);
        }

        public static JsonContent Serialize<T>(this T content)
        {
            return JsonContent.Create(content, new MediaTypeHeaderValue("application/json"), _options);
        }
    }
}