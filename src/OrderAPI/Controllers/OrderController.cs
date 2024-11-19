using Microsoft.AspNetCore.Mvc;
using OrderAPI.Models;
using OrderAPI.Services;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;

        // POST api/order
        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] Order order)
        {
            var createdOrder = await _orderService.CreateOrderAsync(order);
            if (createdOrder == null)
            {
                return BadRequest("Unable to create order.");
            }

            return CreatedAtAction(
                nameof(GetOrderAsync),
                new { id = createdOrder.Id },
                createdOrder
            );
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
