using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ObatAPI.Models;

namespace ObatAPI.Helpers
{
    public class ObatStatusJsonConverter : JsonConverter<ObatStatus>
    {
        public override ObatStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var statusStr = reader.GetString()?.Trim();
            if (string.IsNullOrEmpty(statusStr))
                return ObatStatus.Available;

            // Normalize by removing spaces
            var normalized = statusStr.Replace(" ", "").ToLowerInvariant();

            return normalized switch
            {
                "lowstock" => ObatStatus.LowStock,
                "outofstock" => ObatStatus.OutOfStock,
                "expired" => ObatStatus.Expired,
                _ => ObatStatus.Available
            };
        }

        public override void Write(Utf8JsonWriter writer, ObatStatus value, JsonSerializerOptions options)
        {
            // Write in a format that both database and desktop app can understand
            // Desktop app expects "LowStock", "OutOfStock", etc.
            writer.WriteStringValue(value.ToString());
        }
    }
}
