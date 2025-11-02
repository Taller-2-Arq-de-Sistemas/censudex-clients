
using censudex_clients_service.src.Dtos;

namespace censudex_clients_service.src.Services
{
    public interface IVerifyToken
    {
        /// <summary>
        /// Verifies a JWT token with the AuthService and returns its validation result.
        /// </summary>
        /// <param name="token">The raw JWT token string (without the "Bearer " prefix).</param>
        /// <returns>
        /// Returns a <see cref="ValidateTokenResponse"/> object if the token is valid;
        /// otherwise, returns <c>null</c> if the token is invalid or the AuthService is unreachable.
        /// </returns>
        Task<ValidateTokenResponse?> VerifyTokenAsync(string token);
    }
}