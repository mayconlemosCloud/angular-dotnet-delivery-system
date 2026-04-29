namespace DeliverySystem.Api.Application.DTOs;

public record TrackingJob(
    string OrderNumber,
    string UserId,
    double OriginLat,
    double OriginLon,
    double DestLat,
    double DestLon);
