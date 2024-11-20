using Akka.Actor;
using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderProcessingService.Actors;
using OrderProcessingService.Configurations;
using OrderProcessingService.Consumers;

Env.Load();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (hostContext, services) =>
        {
            // Load and validate environment variables
            var kafkaBootstrapServers =
                Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAPSERVERS")
                ?? throw new Exception("KAFKA_BOOTSTRAPSERVERS not set.");
            var kafkaTopic =
                Environment.GetEnvironmentVariable("KAFKA_TOPIC")
                ?? throw new Exception("KAFKA_TOPIC not set.");
            var kafkaSaslUsername =
                Environment.GetEnvironmentVariable("KAFKA_SASL_USERNAME")
                ?? throw new Exception("KAFKA_SASL_USERNAME not set.");
            var kafkaSaslPassword =
                Environment.GetEnvironmentVariable("KAFKA_SASL_PASSWORD")
                ?? throw new Exception("KAFKA_SASL_PASSWORD not set.");

            // Register services
            services.AddHostedService<OrderEventConsumer>(); // Kafka consumer hosted service

            // Set up Akka.NET actor system
            var actorSystem = ActorSystem.Create("OrderProcessingSystem");

            // Register actors
            services.AddSingleton<IActorRef>(sp =>
                actorSystem.ActorOf(Props.Create(() => new OrderActor()), "orderActor")
            );
            services.AddSingleton<IActorRef>(sp =>
                actorSystem.ActorOf(Props.Create(() => new InventoryActor()), "inventoryActor")
            );

            // Optionally register configuration settings for reuse
            services.AddSingleton(
                new KafkaSettings
                {
                    BootstrapServers = kafkaBootstrapServers,
                    Topic = kafkaTopic,
                    SaslUsername = kafkaSaslUsername,
                    SaslPassword = kafkaSaslPassword,
                }
            );
        }
    )
    .Build();

// Run the host
await host.RunAsync();
