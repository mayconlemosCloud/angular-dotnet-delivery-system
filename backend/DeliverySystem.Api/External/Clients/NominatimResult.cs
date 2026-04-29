using System.Text.Json.Serialization;

namespace DeliverySystem.Api.External.Clients;

public class NominatimResult
{
    [JsonPropertyName("lat")]
    public string Lat { get; set; } = string.Empty;

    [JsonPropertyName("lon")]
    public string Lon { get; set; } = string.Empty;
}
