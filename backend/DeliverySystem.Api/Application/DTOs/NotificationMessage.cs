namespace DeliverySystem.Api.Application.DTOs;

public record NotificationMessage(string Type, string Message, DateTime CreatedAt);
