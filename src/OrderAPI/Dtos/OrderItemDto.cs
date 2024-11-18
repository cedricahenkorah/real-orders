namespace OrderAPI.Dtos;

public class OrderItemDto
{
    public required string ProductId { get; set; }
    public int Quantity { get; set; }
}
