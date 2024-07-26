using System.Text.Json.Serialization;

namespace TrackingSystemAPI.Dto
{
    public class LocationHistoryDto
    {
        [JsonPropertyName("latitude")]
        public decimal Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public decimal Longitude { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}