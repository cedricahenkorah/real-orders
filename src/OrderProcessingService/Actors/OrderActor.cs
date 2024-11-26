using System.Text;
using System.Text.Json;
using Akka.Actor;
using OrderProcessingService.DTOs;

namespace OrderProcessingService.Actors;

public class OrderActor : ReceiveActor
{
    private readonly IActorRef _inventoryActor;
    private readonly HttpClient _httpClient;
    private string _customerId;
    private string _orderId;
    private string _status;
    private List<OrderItemDto> _items;

    public OrderActor(IActorRef inventoryActor, HttpClient httpClient)
    {
        _inventoryActor = inventoryActor;
        _httpClient = httpClient;

        Receive<OrderEventDto>(async orderEvent =>
        {
            Console.WriteLine($"OrderActor {Self.Path} received order {orderEvent.OrderId}");

            _orderId = orderEvent.OrderId;
            _customerId = orderEvent.CustomerId;
            _status = orderEvent.Status;
            _items = orderEvent.Items;

            HandleOrderProcessing();

            await NotifyOrderApi(_orderId, "Processed");
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

    private async Task NotifyOrderApi(string orderId, string status)
    {
        var statusUpdate = new { Id = orderId, Status = status };

        var updateStatusUrl = $"http://localhost:5207/api/order/status/{orderId}";
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(statusUpdate),
            Encoding.UTF8,
            "application/json"
        );

        try
        {
            var response = await _httpClient.PutAsync(updateStatusUrl, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(
                    $"[OrderActor] Successfully notified API for Order: {orderId} with Status: {status}"
                );
            }
            else
            {
                Console.WriteLine(
                    $"[OrderActor] Failed to notify API for Order: {orderId}. HTTP Status: {response.StatusCode}"
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[OrderActor] Error notifying order API for Order: {orderId}. Exception: {ex.Message}"
            );
        }
    }
}
