using OrderAPI.Models;

namespace OrderAPI.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(string id);
        Task<Product> CreateAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(string id);
    }
}
