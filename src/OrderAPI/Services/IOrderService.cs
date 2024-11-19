using OrderAPI.Models;

namespace OrderAPI.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(string id);
        Task<List<Order>> GetAllOrdersAsync();
        Task<bool> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(string id);
    }
}
