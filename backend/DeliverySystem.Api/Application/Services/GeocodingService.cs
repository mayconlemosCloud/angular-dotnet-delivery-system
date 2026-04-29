using System.Globalization;
using DeliverySystem.Api.Domain.Entities;
using DeliverySystem.Api.External.Clients;

namespace DeliverySystem.Api.Application.Services;

public class GeocodingService
{
    private readonly INominatimClient _nominatimClient;
    private readonly ILogger<GeocodingService> _logger;

    public GeocodingService(INominatimClient nominatimClient, ILogger<GeocodingService> logger)
    {
        _nominatimClient = nominatimClient;
        _logger = logger;
    }

    public async Task<(double Lat, double Lon)?> GeocodeAsync(Address address)
    {
        try
        {
            var query = $"{address.Street}, {address.Neighborhood}, {address.City}, {address.State}, Brasil";
            var results = await _nominatimClient.SearchAsync(query, "json", 1, "br");

            if (results.Count == 0)
                return null;

            if (double.TryParse(results[0].Lat, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) &&
                double.TryParse(results[0].Lon, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
            {
                return (lat, lon);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Geocoding failed for {City}/{State}: {Message}", address.City, address.State, ex.Message);
            return null;
        }
    }
}
