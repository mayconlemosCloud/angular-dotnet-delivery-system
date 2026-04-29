using DeliverySystem.Api.Domain.Entities;

namespace DeliverySystem.Api.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(string id);
    Task CreateAsync(User user);
}
