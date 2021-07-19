using MassTransit;

using Microsoft.Extensions.DependencyInjection;

namespace Warehouse.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumers(typeof(Program).Assembly);
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            })
            .AddMassTransitHostedService()
            .AddGenericRequestClient();

            return services;
        }
    }
}