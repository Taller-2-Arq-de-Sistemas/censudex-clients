
namespace censudex_clients_service.src.Dtos
{
    /// <summary>
    /// Represents the response from the AuthService when validating a token.
    /// </summary>
    public class ValidateTokenResponse
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}