using Refit;

namespace DeliverySystem.Api.External.Clients;

public interface ICepClient
{
    [Get("/ws/{cep}/json/")]
    Task<CepResponse> GetAddressByCepAsync(string cep);
}
