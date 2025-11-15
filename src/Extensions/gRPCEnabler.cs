using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Grpc.AspNetCore;

namespace censudex_clients_service.src.Extensions
{
    public static class gRPCEnabler
    {
        /// <summary>
        /// Configures Kestrel to support both HTTP/1.1 and HTTP/2 protocols for gRPC communication.
        /// </summary>
        /// <param name="builder">The web application builder</param>
        /// <returns>The web application builder for chaining</returns>
        /// <remarks>
        /// This extension method enables HTTP/2 support which is required for gRPC services
        /// while maintaining backward compatibility with HTTP/1.1 for REST endpoints.
        /// </remarks>
        public static WebApplicationBuilder EnablegRPC(this WebApplicationBuilder builder)
        {
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5003, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            });

            return builder;
        }

        /// <summary>
        /// Registers gRPC services with the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        /// <remarks>
        /// This method configures the necessary services for gRPC functionality
        /// including serialization, message handling, and service discovery.
        /// </remarks>
        public static IServiceCollection AddgRPCService(this IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                // Optional: Configure gRPC options here
                options.EnableDetailedErrors = true; // Enable in development
                options.MaxReceiveMessageSize = 32 * 1024 * 1024; // 32MB
                options.MaxSendMessageSize = 32 * 1024 * 1024; // 32MB
            });

            return services;
        }


    }
}