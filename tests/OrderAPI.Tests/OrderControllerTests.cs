using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderAPI.Controllers;
using OrderAPI.Models;
using OrderAPI.Services;
using Xunit;

namespace OrderAPI.Tests
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _controller = new OrderController(_mockOrderService.Object);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ValidId_ReturnsOrder()
        {
          
            var orderId = "6489f1f0c2e7c2eaf0b3f6a1";
            var order = new Order
            {
                Id = "6489f1f0c2e7c2eaf0b3f6a1", g
                CustomerId = "12345", 
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = "prod001", Quantity = 2 },
                    new OrderItem { ProductId = "prod002", Quantity = 5 },
                },
                Status =
                    "Pending" 
                ,
            };

            _mockOrderService
                .Setup(service => service.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            var result = await _controller.GetOrderAsync(orderId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<Order>(okResult.Value);
            Assert.Equal(orderId, returnedOrder.Id);
        }

        [Fact]
        public async Task GetOrderByIdAsync_InvalidId_ReturnsNotFound()
        {
           
            var orderId = "1";
            _mockOrderService
                .Setup(service => service.GetOrderByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            var result = await _controller.GetOrderAsync(orderId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateOrderAsync_ValidOrder_ReturnsCreatedOrder()
        {
           
            var order = new Order
            {
                Id = "6489f1f0c2e7c2eaf0b3f6a1", 
                CustomerId = "12345", 
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = "prod001", Quantity = 2 },
                    new OrderItem { ProductId = "prod002", Quantity = 5 },
                },
                Status =
                    "Pending" 
                ,
            };
            _mockOrderService.Setup(service => service.CreateOrderAsync(order)).ReturnsAsync(order);

            var result = await _controller.CreateOrderAsync(order);
          
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedOrder = Assert.IsType<Order>(createdAtResult.Value);
            Assert.Equal(order.Id, returnedOrder.Id);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ReturnsListOfOrders()
        {
           
            var orders = new List<Order>
            {
                new Order
                {
                    Id = "6489f1f0c2e7c2eaf0b3f6a1",
                    CustomerId = "12345",
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = "prod001", Quantity = 2 },
                        new OrderItem { ProductId = "prod002", Quantity = 5 },
                    },
                    Status = "Pending",
                },
                new Order
                {
                    Id = "6489f1f0c2e7c2eaf0b3f6a2",
                    CustomerId = "67890",
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = "prod003", Quantity = 1 },
                        new OrderItem { ProductId = "prod004", Quantity = 3 },
                    },
                    Status = "Completed",
                },
            };

            _mockOrderService.Setup(service => service.GetAllOrdersAsync()).ReturnsAsync(orders);

            var result = await _controller.GetAllOrdersAsync();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrders = Assert.IsType<List<Order>>(okResult.Value);
            Assert.Equal(2, returnedOrders.Count);
        }

        [Fact]
        public async Task DeleteOrderAsync_ValidId_ReturnsNoContent()
        {
            
            var orderId = "1";
            _mockOrderService
                .Setup(service => service.DeleteOrderAsync(orderId))
                .ReturnsAsync(true);

            var result = await _controller.DeleteOrderAsync(orderId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteOrderAsync_InvalidId_ReturnsNotFound()
        {
         
            var orderId = "1";
            _mockOrderService
                .Setup(service => service.DeleteOrderAsync(orderId))
                .ReturnsAsync(false);

            var result = await _controller.DeleteOrderAsync(orderId);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
