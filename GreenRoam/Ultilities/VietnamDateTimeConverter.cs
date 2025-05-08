using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Service.Service
{
    public class VietnamDateTimeConverter : DateTimeConverterBase
    {
        private static readonly TimeZoneInfo vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var dateString = reader.Value.ToString();
            Console.WriteLine($"Parsing date string: {dateString}");

            DateTime dateTime;
            if (DateTime.TryParse(dateString, out dateTime))
            {
                Console.WriteLine($"Parsed DateTime: {dateTime} (Kind: {dateTime.Kind})");
                if (dateTime.Kind == DateTimeKind.Utc || dateString.EndsWith("Z"))
                {
                    dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, vietnamZone);
                    Console.WriteLine($"Converted to Vietnam Time: {dateTime} (Kind: {dateTime.Kind})");
                }
            }
            else if (DateTime.TryParseExact(dateString, "yyyy-MM-ddTHH:mm:ss.fffZ", null, System.Globalization.DateTimeStyles.RoundtripKind, out dateTime))
            {
                Console.WriteLine($"Parsed ISO 8601 DateTime: {dateTime} (Kind: {dateTime.Kind})");
                dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, vietnamZone);
                Console.WriteLine($"Converted ISO 8601 to Vietnam Time: {dateTime} (Kind: {dateTime.Kind})");
            }
            else
            {
                throw new JsonSerializationException($"Unable to parse date string: {dateString}");
            }

            return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dateTime = (DateTime)value;
            writer.WriteValue(dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff")); // Định dạng không thêm Z
        }
    }
}