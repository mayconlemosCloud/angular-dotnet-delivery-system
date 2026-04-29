namespace DeliverySystem.Api.Application.DTOs;

public record CourierLocationUpdate(
    string OrderNumber,
    double Latitude,
    double Longitude,
    double Progress,
    int EstimatedMinutes,
    bool IsCompleted);
