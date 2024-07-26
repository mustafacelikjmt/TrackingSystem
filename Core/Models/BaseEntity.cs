using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Core.Models
{
    public class BaseEntity
    {
        //public int Id { get; set; }
        [JsonPropertyName("timestamp")]
        [Newtonsoft.Json.JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime Timestamp { get; set; }
    }
}

public class CustomDateTimeConverter : IsoDateTimeConverter
{
    public CustomDateTimeConverter()
    {
        DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
    }
}