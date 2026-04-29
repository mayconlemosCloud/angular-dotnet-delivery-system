namespace DeliverySystem.Api.Application.DTOs;

public record CreateDeliveryRequest(string OrderNumber, DateTime DeliveryDateTime);
