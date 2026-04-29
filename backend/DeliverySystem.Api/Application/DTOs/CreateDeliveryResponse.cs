namespace DeliverySystem.Api.Application.DTOs;

public record CreateDeliveryResponse(
    string Id,
    string OrderNumber,
    DateTime DeliveryDateTime,
    DateTime CreatedAt);
