namespace OrderAPI.Dtos;

public class CreateOrderRequestDto
{
    public required string CustomerId { get; set; }
    public List<OrderItemDto> Items { get; set; }
}
