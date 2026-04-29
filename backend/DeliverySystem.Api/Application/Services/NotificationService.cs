using AutoMapper;
using DeliverySystem.Api.Application.DTOs;
using DeliverySystem.Api.Domain.Entities;
using DeliverySystem.Api.Domain.Interfaces;

namespace DeliverySystem.Api.Application.Services;

public class NotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;

    public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
    }

    public async Task<Notification> CreateAsync(string userId, string type, string message)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            Message = message
        };

        await _notificationRepository.CreateAsync(notification);
        return notification;
    }

    public async Task<IEnumerable<NotificationResponse>> GetByUserIdAsync(string userId)
    {
        var notifications = await _notificationRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<NotificationResponse>>(notifications);
    }

    public async Task<bool> MarkAsReadAsync(string notificationId, string userId)
    {
        var notifications = await _notificationRepository.GetByUserIdAsync(userId);
        var exists = notifications.Any(n => n.Id == notificationId);
        if (!exists) return false;

        await _notificationRepository.MarkAsReadAsync(notificationId, userId);
        return true;
    }
}
