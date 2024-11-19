using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderAPI.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("customerId")]
    public string CustomerId { get; set; }

    [BsonElement("items")]
    public List<OrderItem> Items { get; set; }

    [BsonElement("status")]
    public string Status { get; set; }
}
