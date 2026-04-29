using DeliverySystem.Api.Domain.Entities;

namespace DeliverySystem.Api.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
    Task CreateAsync(Order order);
}
