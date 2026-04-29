namespace DeliverySystem.Api.Application.DTOs;

public record AddressRequest(
    string ZipCode,
    string Street,
    string Number,
    string Neighborhood,
    string City,
    string State);
