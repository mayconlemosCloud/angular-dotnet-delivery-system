using DeliverySystem.Api.Domain.Entities;
using DeliverySystem.Api.Domain.Interfaces;
using MongoDB.Driver;

namespace DeliverySystem.Api.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<Notification> _collection;

    public NotificationRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Notification>("notifications");
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId)
    {
        return await _collection
            .Find(n => n.UserId == userId)
            .SortByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task CreateAsync(Notification notification)
    {
        await _collection.InsertOneAsync(notification);
    }

    public async Task MarkAsReadAsync(string notificationId, string userId)
    {
        var filter = Builders<Notification>.Filter.And(
            Builders<Notification>.Filter.Eq(n => n.Id, notificationId),
            Builders<Notification>.Filter.Eq(n => n.UserId, userId));

        var update = Builders<Notification>.Update.Set(n => n.IsRead, true);

        await _collection.UpdateOneAsync(filter, update);
    }
}
