using OrderAPI.Dtos;
using OrderAPI.Models;

namespace OrderAPI.Services
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(string id);
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> CreateProductAsync(CreateProductDto createProductDto);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(string id);
    }
}
