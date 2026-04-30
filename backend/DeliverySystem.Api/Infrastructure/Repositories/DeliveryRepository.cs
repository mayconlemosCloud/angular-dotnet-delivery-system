using DeliverySystem.Api.Domain.Entities;
using DeliverySystem.Api.Domain.Interfaces;
using MongoDB.Driver;

namespace DeliverySystem.Api.Infrastructure.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly IMongoCollection<Delivery> _collection;

    public DeliveryRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Delivery>("deliveries");
    }

    public async Task<Delivery?> GetByOrderNumberAsync(string orderNumber)
        => await _collection.Find(d => d.OrderNumber == orderNumber).FirstOrDefaultAsync();

    public async Task<IEnumerable<Delivery>> GetByUserIdAsync(string userId)
        => await _collection.Find(d => d.UserId == userId).ToListAsync();

    public async Task CreateAsync(Delivery delivery)
        => await _collection.InsertOneAsync(delivery);

    public async Task UpdateStatusAsync(string orderNumber, string status)
    {
        var filter = Builders<Delivery>.Filter.Eq(d => d.OrderNumber, orderNumber);
        var update = Builders<Delivery>.Update.Set(d => d.Status, status);
        await _collection.UpdateOneAsync(filter, update);
    }
}
