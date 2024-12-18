using Akka.Actor;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OrderProcessingService.DTOs;

namespace OrderProcessingService.Consumers;

public class OrderEventConsumer : IHostedService
{
    private readonly string _topic =
        Environment.GetEnvironmentVariable("KAFKA_TOPIC")
        ?? throw new Exception("KAFKA_TOPIC not set.");
    private readonly IConsumer<string, string> _consumer;
    private readonly IActorRef _orderManagerActor;
    private CancellationTokenSource _cancellationTokenSource;

    public OrderEventConsumer(IActorRef orderManagerActor)
    {
        _orderManagerActor = orderManagerActor;

        var config = new ConsumerConfig
        {
            BootstrapServers =
                Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAPSERVERS")
                ?? throw new Exception("KAFKA_BOOTSTRAPSERVERS not set."),
            SaslUsername =
                Environment.GetEnvironmentVariable("KAFKA_SASL_USERNAME")
                ?? throw new Exception("KAFKA_SASL_USERNAME not set."),
            SaslPassword =
                Environment.GetEnvironmentVariable("KAFKA_SASL_PASSWORD")
                ?? throw new Exception("KAFKA_SASL_PASSWORD not set."),
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            GroupId = "real-orders",
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken
        );

        Task.Run(() => StartConsuming(_cancellationTokenSource.Token), cancellationToken);

        Console.WriteLine("OrderEventConsumer started.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stopping OrderEventConsumer...");
        _cancellationTokenSource.Cancel();
        _consumer.Close();
        return Task.CompletedTask;
    }

    private void StartConsuming(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    var orderEvent = JsonConvert.DeserializeObject<OrderEventDto>(
                        consumeResult.Message.Value
                    );

                    Console.WriteLine($"Received event: {JsonConvert.SerializeObject(orderEvent)}");

                    HandleOrderEvent(orderEvent);
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Consume error: {e.Error.Reason}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Consumer loop cancelled.");
        }
        finally
        {
            _consumer.Close();
        }
    }

    private void HandleOrderEvent(OrderEventDto orderEvent)
    {
        Console.WriteLine($"Forwarding order {orderEvent.OrderId} to OrderManagerActor...");
        _orderManagerActor.Tell(orderEvent);
    }
}
