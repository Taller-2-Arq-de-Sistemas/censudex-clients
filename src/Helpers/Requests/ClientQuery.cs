namespace censudex_clients_service.src.Helpers.Requests
{
    public class ClientQuery
    {
        public string? FullName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Username { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = null;
        public string? SortBy { get; set; } = string.Empty;
        public bool IsDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}