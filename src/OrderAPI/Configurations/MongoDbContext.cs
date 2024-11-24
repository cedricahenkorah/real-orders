using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderAPI.Models;

namespace OrderAPI.Configurations
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
            _database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        }

        public IMongoCollection<Order> Orders => _database.GetCollection<Order>("Orders");
        public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");

        public bool TestConnection()
        {
            try
            {
                // List collections as a test query
                _database.ListCollections();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
