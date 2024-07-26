using System.Text.Json.Serialization;

namespace Core.Models
{
    public class LocationModel : BaseEntity
    {
        [JsonPropertyName("latitude")]
        public decimal Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public decimal Longitude { get; set; }
    }
}