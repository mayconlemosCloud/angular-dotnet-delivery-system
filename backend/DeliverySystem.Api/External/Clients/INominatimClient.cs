using Refit;

namespace DeliverySystem.Api.External.Clients;

public interface INominatimClient
{
    [Get("/search")]
    Task<List<NominatimResult>> SearchAsync(
        [AliasAs("q")] string query,
        [AliasAs("format")] string format,
        [AliasAs("limit")] int limit,
        [AliasAs("countrycodes")] string countryCodes);
}
