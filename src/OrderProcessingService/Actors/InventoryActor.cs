using Akka.Actor;
using OrderProcessingService.DTOs;

namespace OrderProcessingService.Actors;

public class InventoryActor : ReceiveActor
{
    public InventoryActor()
    {
        Receive<OrderEventDto>(orderEvent =>
        {
            CheckInventoryForItems(orderEvent.Items);
        });
    }

    private static void CheckInventoryForItems(List<OrderItemDto> items)
    {
        // Simulate inventory check
        foreach (var item in items)
        {
            Console.WriteLine(
                $"Checking inventory for Product: {item.ProductId} with Quantity: {item.Quantity}"
            );
        }
    }
}
