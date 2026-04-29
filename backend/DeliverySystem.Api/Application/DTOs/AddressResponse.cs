namespace DeliverySystem.Api.Application.DTOs;

public record AddressResponse(
    string ZipCode,
    string Street,
    string Number,
    string Neighborhood,
    string City,
    string State);
