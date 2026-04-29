using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DeliverySystem.Api.Domain.Entities;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string OrderNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Value { get; set; }

    public Address DeliveryAddress { get; set; } = new();

    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
