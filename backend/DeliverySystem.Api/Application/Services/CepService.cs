using DeliverySystem.Api.Domain.Entities;
using DeliverySystem.Api.External.Clients;

namespace DeliverySystem.Api.Application.Services;

public class CepService
{
    private readonly ICepClient _cepClient;

    public CepService(ICepClient cepClient)
    {
        _cepClient = cepClient;
    }

    public async Task<Address?> GetAddressAsync(string zipCode, string number)
    {
        try
        {
            var cleanCep = zipCode.Replace("-", "").Trim();
            var response = await _cepClient.GetAddressByCepAsync(cleanCep);

            if (response.IsInvalid)
                return null;

            return new Address
            {
                ZipCode = zipCode,
                Street = response.Logradouro,
                Number = number,
                Neighborhood = response.Bairro,
                City = response.Localidade,
                State = response.Uf
            };
        }
        catch
        {
            return null;
        }
    }
}
