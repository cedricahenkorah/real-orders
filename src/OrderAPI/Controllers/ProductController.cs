using Microsoft.AspNetCore.Mvc;
using OrderAPI.Dtos;
using OrderAPI.Models;
using OrderAPI.Services;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            var createdProduct = await _productService.CreateProductAsync(createProductDto);

            if (createdProduct == null)
            {
                return BadRequest("Unable to create product");
            }

            return Ok(createdProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] Product product)
        {
            product.Id = id;

            var updatedProduct = await _productService.UpdateProductAsync(product);

            if (!updatedProduct)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var deletedProduct = await _productService.DeleteProductAsync(id);

            if (!deletedProduct)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
