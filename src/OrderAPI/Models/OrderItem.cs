using MongoDB.Bson.Serialization.Attributes;

namespace OrderAPI.Models;

public class OrderItem
{
    [BsonElement("productId")]
    public required string ProductId { get; set; }

    [BsonElement("quantity")]
    public int Quantity { get; set; }
}
