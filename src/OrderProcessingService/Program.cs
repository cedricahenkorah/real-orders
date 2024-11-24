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

            // Set up Akka.NET actor system
            var actorSystem = ActorSystem.Create("OrderProcessingSystem");
            services.AddSingleton(actorSystem);

            // Register ActorProvider
            services.AddSingleton<ActorProvider>(sp =>
            {
                var actorSystem = sp.GetRequiredService<ActorSystem>();
                return new ActorProvider(actorSystem);
            });

            // Register OrderManagerActor via ActorProvider
            services.AddSingleton<IActorRef>(sp =>
            {
                var actorProvider = sp.GetRequiredService<ActorProvider>();
                return actorProvider.GetOrderManagerActor();
            });

            // Register Kafka settings for reuse
            services.AddSingleton(
                new KafkaSettings
                {
                    BootstrapServers = kafkaBootstrapServers,
                    Topic = kafkaTopic,
                    SaslUsername = kafkaSaslUsername,
                    SaslPassword = kafkaSaslPassword,
                }
            );

            // Register the Kafka consumer and pass the OrderManagerActor
            services.AddHostedService(sp =>
            {
                var orderManagerActor = sp.GetRequiredService<IActorRef>(); // OrderManagerActor
                return new OrderEventConsumer(orderManagerActor);
            });
        }
    )
    .Build();

await host.RunAsync();
