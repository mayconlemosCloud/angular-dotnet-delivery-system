using DeliverySystem.Api.Domain.Entities;

namespace DeliverySystem.Api.Domain.Interfaces;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetByUserIdAsync(string userId);
    Task CreateAsync(Notification notification);
    Task MarkAsReadAsync(string notificationId, string userId);
}
