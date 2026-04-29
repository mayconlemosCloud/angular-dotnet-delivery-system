using DeliverySystem.Api.Domain.Entities;

namespace DeliverySystem.Api.Domain.Interfaces;

public interface IDeliveryRepository
{
    Task<IEnumerable<Delivery>> GetByUserIdAsync(string userId);
    Task CreateAsync(Delivery delivery);
}
