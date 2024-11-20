using OrderAPI.Dtos;
using OrderAPI.Models;
using OrderAPI.Repositories;

namespace OrderAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderRequestDto createOrderDto)
        {
            var order = new Order
            {
                CustomerId = createOrderDto.CustomerId,
                Items = createOrderDto
                    .Items.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                    })
                    .ToList(),
                Status = "Pending",
            };

            return await _orderRepository.CreateAsync(order);
        }

        public async Task<Order> GetOrderByIdAsync(string id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            return await _orderRepository.UpdateAsync(order);
        }

        public async Task<bool> DeleteOrderAsync(string id)
        {
            return await _orderRepository.DeleteAsync(id);
        }
    }
}
