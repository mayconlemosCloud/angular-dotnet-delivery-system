using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DeliverySystem.Api.Domain.Entities;

public class Delivery
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string OrderNumber { get; set; } = string.Empty;
    public DateTime DeliveryDateTime { get; set; }
    public string Status { get; set; } = DeliveryStatus.InRoute;

    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public static class DeliveryStatus
{
    public const string InRoute   = "IN_ROUTE";
    public const string Delivered = "DELIVERED";
}
