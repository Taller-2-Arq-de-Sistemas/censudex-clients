

namespace censudex_clients_service.src.Dtos
{
    public class ViewUserResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        
    }
}