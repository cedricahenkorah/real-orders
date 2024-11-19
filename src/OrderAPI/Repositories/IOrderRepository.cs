using OrderAPI.Models;

namespace OrderAPI.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order);
        Task<Order> GetByIdAsync(string id);
        Task<List<Order>> GetAllAsync();
        Task<bool> UpdateAsync(Order order);
        Task<bool> DeleteAsync(string id);
    }
}
