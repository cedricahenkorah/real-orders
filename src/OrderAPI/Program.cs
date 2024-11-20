using DotNetEnv;
using OrderAPI.Configurations;
using OrderAPI.Repositories;
using OrderAPI.Services;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bind the "MongoDB" section of appsettings.json to MongoDbSettings
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));

// Replace placeholders with environment variables
var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
var mongoDatabaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME");
var kafkaBootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAPSERVERS");
var kafkaTopic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
var kafkaSaslUsername = Environment.GetEnvironmentVariable("KAFKA_SASL_USERNAME");
var kafkaSaslPassword = Environment.GetEnvironmentVariable("KAFKA_SASL_PASSWORD");

builder.Configuration["MongoDB:ConnectionString"] =
    mongoConnectionString ?? throw new Exception("MONGO_CONNECTION_STRING not set.");
builder.Configuration["MongoDB:DatabaseName"] =
    mongoDatabaseName ?? throw new Exception("MONGO_DATABASE_NAME not set.");
builder.Configuration["Kafka:BootstrapServers"] =
    kafkaBootstrapServers ?? throw new Exception("KAFKA_BOOTSTRAPSERVERS not set.");
builder.Configuration["Kafka:Topic"] = kafkaTopic ?? throw new Exception("KAFKA_TOPIC not set.");
builder.Configuration["Kafka:SaslUsername"] =
    kafkaSaslUsername ?? throw new Exception("KAFKA_SASL_USERNAME not set.");
builder.Configuration["Kafka:SaslPassword"] =
    kafkaSaslPassword ?? throw new Exception("KAFKA_SASL_PASSWORD not set.");

// Register MongoDB context
builder.Services.AddSingleton<MongoDbContext>();

// Register services and repositories
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map controllers to routes
app.MapControllers();

// Test MongoDB connection
using (var scope = app.Services.CreateScope())
{
    var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    try
    {
        if (mongoDbContext.TestConnection())
        {
            Console.WriteLine("MongoDB connection successful.");
        }
        else
        {
            Console.WriteLine("MongoDB connection failed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error testing MongoDB connection: {ex.Message}");
    }
}

app.Run();
