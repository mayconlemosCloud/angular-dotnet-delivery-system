namespace DeliverySystem.Api.Application.DTOs;

public record CreateOrderResponse(
    string Id,
    string OrderNumber,
    string Description,
    decimal Value,
    AddressResponse DeliveryAddress,
    DateTime CreatedAt);
