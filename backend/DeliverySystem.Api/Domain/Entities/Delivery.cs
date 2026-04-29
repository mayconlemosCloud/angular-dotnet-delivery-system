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

    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
