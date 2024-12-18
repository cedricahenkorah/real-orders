using MongoDB.Driver;
using OrderAPI.Configurations;
using OrderAPI.Models;

namespace OrderAPI.Repositories
{
    public class OrderRepository(MongoDbContext dbContext) : IOrderRepository
    {
        private readonly IMongoCollection<Order> _orders = dbContext.Orders;

        public async Task<Order> CreateAsync(Order order)
        {
            await _orders.InsertOneAsync(order);
            return order;
        }

        public async Task<Order> GetByIdAsync(string id)
        {
            return await _orders.Find(order => order.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _orders.Find(order => true).ToListAsync();
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            var result = await _orders.ReplaceOneAsync(o => o.Id == order.Id, order);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Order Id must be provided.");

            var result = await _orders.DeleteOneAsync(order => order.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<bool> UpdateOrderStatusAsync(string id, string status)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Order Id and status must be provided.");

            var result = await _orders.UpdateOneAsync(
                o => o.Id == id,
                Builders<Order>.Update.Set(o => o.Status, status)
            );

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
