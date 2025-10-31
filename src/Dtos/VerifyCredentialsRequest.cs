

namespace censudex_clients_service.src.Dtos
{
    /// <summary>
    /// Represents the data required to verify client credentials.
    /// </summary>
    public class VerifyCredentialsRequest
    {
        /// <summary>
        /// The client's email address (optional if username is provided).
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// The client's username (optional if email is provided).
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// The client's password (required).
        /// </summary>
        public string Password { get; set; } = string.Empty;

    }
}