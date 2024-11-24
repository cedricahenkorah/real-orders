using Akka.Actor;

namespace OrderProcessingService.Actors;

public class ActorProvider
{
    private readonly ActorSystem _actorSystem;
    private readonly IActorRef _inventoryActor;

    public ActorProvider(ActorSystem actorSystem)
    {
        _actorSystem = actorSystem;

        // Create InventoryActor when ActorProvider is initialized
        _inventoryActor = _actorSystem.ActorOf(
            Props.Create(() => new InventoryActor()),
            "inventoryActor"
        );
    }

    public IActorRef GetOrderManagerActor()
    {
        return _actorSystem.ActorOf(
            Props.Create(() => new OrderManagerActor(_inventoryActor)),
            "orderManagerActor"
        );
    }
}
