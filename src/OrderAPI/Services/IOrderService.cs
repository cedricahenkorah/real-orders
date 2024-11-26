using OrderAPI.Dtos;
using OrderAPI.Models;

namespace OrderAPI.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(CreateOrderRequestDto createOrderDto);
        Task<Order> GetOrderByIdAsync(string id);
        Task<List<Order>> GetAllOrdersAsync();
        Task<bool> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(string id);
        Task<bool> UpdateOrderStatusAsync(string id, string status);
    }
}
