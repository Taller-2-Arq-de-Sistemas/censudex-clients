
using System.Net.Http.Headers;
using censudex_clients_service.src.Dtos;

namespace censudex_clients_service.src.Services
{
    public class VerifyToken : IVerifyToken
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<VerifyToken> _logger;
        private readonly string _authBaseUrl;

        public VerifyToken(HttpClient httpClient, ILogger<VerifyToken> logger, IConfiguration config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _authBaseUrl = config["AUTH_SERVICE_URL"] ?? throw new InvalidOperationException("AUTH_SERVICE_URL environment variable is not set");
        
        }

        /// <summary>
        /// Verifies if JWT token is still valid within the AuthService and returns its validation result.
        /// </summary>
        /// <param name="token">The raw JWT token string (without the "Bearer " prefix).</param>
        /// <returns>
        /// Returns a <see cref="ValidateTokenResponse"/> object if the token is valid;
        /// otherwise, returns <c>null</c> if the token is invalid or the AuthService is unreachable.
        /// </returns>
        public async Task<ValidateTokenResponse?> VerifyTokenAsync(string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_authBaseUrl}/auth/validate-token");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Token validation failed with status code: {StatusCode}", response.StatusCode);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<ValidateTokenResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed: {ErrorMessage}", ex.Message);
                return null;
            }
        }
    }
}