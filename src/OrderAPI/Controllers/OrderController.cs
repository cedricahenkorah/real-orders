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
            if (createOrderDto == null)
            {
                return BadRequest("Request body cannot be null. Unable to create order.");
            }

            try
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
                try
                {
                    await _kafkaProducer.ProduceAsync(
                        "order-events",
                        createdOrder.Id,
                        JsonSerializer.Serialize(orderEvent)
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to publish OrderCreated event to Kafka. {ex}");
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        "Order created but event publishing failed."
                    );
                }

                return Ok(createdOrder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while creating order. {ex}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request."
                );
            }
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

        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateOrderStatusAsync(
            string id,
            [FromBody] UpdateOrderStatusDto updateOrderStatusDto
        )
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = updateOrderStatusDto.Status;

            var success = await _orderService.UpdateOrderAsync(order);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
