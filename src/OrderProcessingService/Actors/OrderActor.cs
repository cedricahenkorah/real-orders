using Akka.Actor;
using OrderProcessingService.DTOs;

namespace OrderProcessingService.Actors;

public class OrderActor : ReceiveActor
{
    private readonly IActorRef _inventoryActor;

    private string _customerId;
    private string _orderId;
    private string _status;
    private List<OrderItemDto> _items;

    public OrderActor(IActorRef inventoryActor)
    {
        _inventoryActor = inventoryActor;

        Receive<OrderEventDto>(orderEvent =>
        {
            Console.WriteLine($"OrderActor {Self.Path} received order {orderEvent.OrderId}");

            _orderId = orderEvent.OrderId;
            _customerId = orderEvent.CustomerId;
            _status = orderEvent.Status;
            _items = orderEvent.Items;

            HandleOrderProcessing();
        });
    }

    private void HandleOrderProcessing()
    {
        Console.WriteLine($"Processing Order: {_orderId} with {_items?.Count ?? 0} items.");

        if (_items != null && _items.Count > 0)
        {
            // Forward inventory check to InventoryActor
            _inventoryActor.Tell(
                new OrderEventDto
                {
                    OrderId = _orderId,
                    CustomerId = _customerId,
                    Items = _items,
                    Status = _status,
                }
            );
        }
    }
}
