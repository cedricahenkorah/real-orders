namespace OrderProcessingService.DTOs;

public class OrderEventDto
{
    public string OrderId { get; set; }
    public string CustomerId { get; set; }
    public List<OrderItemDto> Items { get; set; }
    public string Status { get; set; }
}
