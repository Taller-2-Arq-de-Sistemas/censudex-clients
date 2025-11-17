
using MassTransit;

namespace censudex_clients_service.src.Extensions
{
    public static class MassTransitConfig
    {
        public static IServiceCollection AddRabbitMqWithMassTransit(this IServiceCollection services, IConfiguration config)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(
                        config["RABBITMQ_HOST"], "/", h =>
                        {
                            h.Username(config["RABBITMQ_USER"] ?? throw new ArgumentNullException("RABBITMQ_USER", "RabbitMQ username cannot be null"));
                            h.Password(config["RABBITMQ_PASS"] ?? throw new ArgumentNullException("RABBITMQ_PASS", "RabbitMQ username cannot be null"));
                        }
                    );

                    cfg.MessageTopology.SetEntityNameFormatter(new CustomEntityNameFormatter());
                });
            });

            return services;
        }
    }

    // Optional: Keep exchange names clean
    public class CustomEntityNameFormatter : IEntityNameFormatter
    {
        public string FormatEntityName<T>()
        {
            return "censudex.events";
        }

    }
}