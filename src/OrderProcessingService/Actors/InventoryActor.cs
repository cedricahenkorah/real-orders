using System.Text;
using System.Text.Json;
using Akka.Actor;
using OrderProcessingService.DTOs;

namespace OrderProcessingService.Actors;

public class InventoryActor : ReceiveActor
{
    private readonly HttpClient _httpClient;

    public InventoryActor(HttpClient httpClient)
    {
        _httpClient = httpClient;

        Receive<OrderEventDto>(async orderEvent =>
        {
            CheckInventoryForItems(orderEvent.Items);

            // Notify OrderAPI about the inventory check result
            await NotifyOrderStatus(orderEvent.OrderId, "InventoryChecked");
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

    private async Task NotifyOrderStatus(string orderId, string status)
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
                    $"[InventoryActor] Successfully notified API for Order: {orderId} with Status: {status}"
                );
            }
            else
            {
                Console.WriteLine(
                    $"[InventoryActor] Failed to notify API for Order: {orderId}. HTTP Status: {response.StatusCode}"
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[InventoryActor] Error notifying order API for Order: {orderId}. Exception: {ex.Message}"
            );
        }
    }
}
