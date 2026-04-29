namespace DeliverySystem.Api.Application.DTOs;

public record CreateOrderRequest(
    string Description,
    decimal Value,
    AddressRequest DeliveryAddress);
