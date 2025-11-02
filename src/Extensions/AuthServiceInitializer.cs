
using System.Net.Http.Headers;
using censudex_clients_service.src.Services;

namespace censudex_clients_service.src.Extensions
{
    public static class AuthServiceInitializer
    {
        /// <summary>
        /// Registers the Auth Service HTTP client and related dependencies.
        /// </summary>
        /// <param name="services">The IServiceCollection to add the client to.</param>
        /// <returns>The updated IServiceCollection for chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown if AUTH_SERVICE_URL is not configured.</exception>
        public static IServiceCollection AddAuthServiceClient(this IServiceCollection services)
        {
            var authServiceUrl = Environment.GetEnvironmentVariable("AUTH_SERVICE_URL")
                ?? throw new InvalidOperationException("AUTH_SERVICE_URL environment variable is not set");

            services.AddHttpClient<IVerifyToken, VerifyToken>(client =>
            {
                client.BaseAddress = new Uri(authServiceUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            return services;
        }
    }
}