using AutoMapper;
using DeliverySystem.Api.Application.DTOs;
using DeliverySystem.Api.Domain.Entities;
using DeliverySystem.Api.Domain.Interfaces;
using DeliverySystem.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DeliverySystem.Api.Application.Services;

public class NotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IMapper _mapper;

    public NotificationService(
        INotificationRepository notificationRepository,
        IHubContext<NotificationHub> hubContext,
        IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
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

        var notificationMessage = new NotificationMessage(type, message, notification.CreatedAt);
        await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notificationMessage);

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
