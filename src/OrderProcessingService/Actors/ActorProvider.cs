using Akka.Actor;

namespace OrderProcessingService.Actors;

public class ActorProvider
{
    private readonly ActorSystem _actorSystem;
    private readonly IActorRef _inventoryActor;
    private readonly HttpClient _httpClient;

    public ActorProvider(ActorSystem actorSystem, HttpClient httpClient)
    {
        _actorSystem = actorSystem;
        _httpClient = httpClient;

        // Create InventoryActor when ActorProvider is initialized
        _inventoryActor = _actorSystem.ActorOf(
            Props.Create(() => new InventoryActor(_httpClient)),
            "inventoryActor"
        );
    }

    public IActorRef GetOrderManagerActor()
    {
        return _actorSystem.ActorOf(
            Props.Create(() => new OrderManagerActor(_inventoryActor, _httpClient)),
            "orderManagerActor"
        );
    }
}
