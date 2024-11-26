using Akka.Actor;
using OrderProcessingService.DTOs;

namespace OrderProcessingService.Actors;

public class OrderManagerActor : ReceiveActor
{
    private readonly IActorRef _inventoryActor;
    private readonly HttpClient _httpClient;

    public OrderManagerActor(IActorRef inventoryActor, HttpClient httpClient)
    {
        _inventoryActor = inventoryActor;
        _httpClient = httpClient;

        Receive<OrderEventDto>(orderEvent =>
        {
            Console.WriteLine($"OrderManagerActor received order {orderEvent.OrderId}");

            // Ensure unique actor name per order
            var orderActorName = $"order-{orderEvent.OrderId}";

            // Check if the actor exists; if not, create it
            var orderActor = Context
                .Child(orderActorName)
                .GetOrElse(() =>
                {
                    Console.WriteLine($"Creating OrderActor for order {orderEvent.OrderId}");
                    return Context.ActorOf(
                        Props.Create(() => new OrderActor(_inventoryActor, _httpClient)),
                        orderActorName
                    );
                });

            // Forward the order event to the OrderActor
            Console.WriteLine($"Forwarding order {orderEvent.OrderId} to OrderActor.");
            orderActor.Tell(orderEvent);
        });
    }
}
