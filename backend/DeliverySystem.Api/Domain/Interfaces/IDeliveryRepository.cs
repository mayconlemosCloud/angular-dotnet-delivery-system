using DeliverySystem.Api.Domain.Entities;

namespace DeliverySystem.Api.Domain.Interfaces;

public interface IDeliveryRepository
{
    Task<Delivery?> GetByOrderNumberAsync(string orderNumber);
    Task<IEnumerable<Delivery>> GetByUserIdAsync(string userId);
    Task CreateAsync(Delivery delivery);
}
