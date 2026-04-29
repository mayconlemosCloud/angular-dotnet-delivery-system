namespace DeliverySystem.Api.Application.DTOs;

public record NotificationResponse(
    string Id,
    string Type,
    string Message,
    bool IsRead,
    DateTime CreatedAt);
