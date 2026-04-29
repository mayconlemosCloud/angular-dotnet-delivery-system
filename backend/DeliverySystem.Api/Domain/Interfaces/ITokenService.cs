using DeliverySystem.Api.Domain.Entities;

namespace DeliverySystem.Api.Domain.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
