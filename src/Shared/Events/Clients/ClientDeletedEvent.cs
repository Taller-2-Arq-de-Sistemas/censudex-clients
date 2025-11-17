
namespace censudex_clients_service.src.Shared.Events.Clients
{
    public class ClientDeletedEvent
    {
        public required string Id { get; set; }
        public required DateTime DeletedAt { get; set; }

    }
}