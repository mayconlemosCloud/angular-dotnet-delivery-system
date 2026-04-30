namespace DeliverySystem.Api.Application.DTOs;

public record CreateDeliveryResponse(
    string Id,
    string OrderNumber,
    DateTime DeliveryDateTime,
    string Status,
    DateTime CreatedAt);
