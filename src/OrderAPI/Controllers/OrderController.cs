using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Dtos;
using OrderAPI.Models;
using OrderAPI.Services;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IOrderService orderService, IKafkaProducer kafkaProducer)
        : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;
        private readonly IKafkaProducer _kafkaProducer = kafkaProducer;

        // POST api/order
        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync(
            [FromBody] CreateOrderRequestDto createOrderDto
        )
        {
            var createdOrder = await _orderService.CreateOrderAsync(createOrderDto);
            if (createdOrder == null)
            {
                return BadRequest("Unable to create order.");
            }

            // Publish the "OrderCreated" event to Kafka
            var orderEvent = new
            {
                EventType = "OrderCreated",
                OrderId = createdOrder.Id,
                CustomerId = createdOrder.CustomerId,
                Items = createdOrder.Items,
                Status = "Created",
                Timestamp = DateTime.UtcNow,
            };

            await _kafkaProducer.ProduceAsync(
                "order-events",
                createdOrder.Id,
                JsonSerializer.Serialize(orderEvent)
            );

            return Ok(createdOrder);
        }

        // GET api/order/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderAsync(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // GET api/order
        [HttpGet]
        public async Task<IActionResult> GetAllOrdersAsync()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // PUT api/order/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderAsync(string id, [FromBody] Order order)
        {
            order.Id = id;

            var success = await _orderService.UpdateOrderAsync(order);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE api/order/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderAsync(string id)
        {
            var success = await _orderService.DeleteOrderAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
