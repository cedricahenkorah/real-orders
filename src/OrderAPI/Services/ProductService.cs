using OrderAPI.Dtos;
using OrderAPI.Models;
using OrderAPI.Repositories;

namespace OrderAPI.Services
{
    public class ProductService(IProductRepository productRepository) : IProductService
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<Product> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = new Product
            {
                Name = createProductDto.Name,
                Price = createProductDto.Price,
                Quantity = createProductDto.Quantity,
            };

            return await _productRepository.CreateAsync(product);
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            return await _productRepository.DeleteProductAsync(id);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetProductsAsync();
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            return await _productRepository.GetProductByIdAsync(id);
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            return await _productRepository.UpdateProductAsync(product);
        }
    }
}
