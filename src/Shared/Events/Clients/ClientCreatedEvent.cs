
namespace censudex_clients_service.src.Shared.Events.Clients
{
    public class ClientCreatedEvent
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string FullName { get; set; }
        public required string PhoneNumber { get; set; }
        public required DateTime CreatedAt { get; set; }

    }
}