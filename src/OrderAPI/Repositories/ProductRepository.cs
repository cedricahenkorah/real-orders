using MongoDB.Driver;
using OrderAPI.Configurations;
using OrderAPI.Models;

namespace OrderAPI.Repositories
{
    public class ProductRepository(MongoDbContext mongoDbContext) : IProductRepository
    {
        private readonly IMongoCollection<Product> _products = mongoDbContext.Products;

        public async Task<Product> CreateAsync(Product product)
        {
            await _products.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            var result = await _products.DeleteOneAsync(order => order.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            return await _products.Find(product => product.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _products.Find(product => true).ToListAsync();
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var result = await _products.ReplaceOneAsync(order => order.Id == product.Id, product);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
