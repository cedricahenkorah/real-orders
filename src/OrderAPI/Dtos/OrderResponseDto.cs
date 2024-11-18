namespace OrderAPI.Dtos;

public class OrderResponseDto
{
    public string OrderId { get; set; }
    public string Status { get; set; }
    public List<OrderItemDto> Items { get; set; }
}
