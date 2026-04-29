using DeliverySystem.Api.Domain.Entities;
using DeliverySystem.Api.Domain.Interfaces;
using MongoDB.Driver;

namespace DeliverySystem.Api.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _collection;

    public OrderRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Order>("orders");
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await _collection.Find(o => o.OrderNumber == orderNumber).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
    {
        return await _collection.Find(o => o.UserId == userId).ToListAsync();
    }

    public async Task CreateAsync(Order order)
    {
        await _collection.InsertOneAsync(order);
    }
}
