
using MassTransit;

using Microsoft.Extensions.DependencyInjection;

using Sales.Application.Orders;
using Sales.Application.Subscriptions;

namespace Sales.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //services.AddMediatR(typeof(ServiceCollectionExtensions));

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

            services.AddOrders();
            services.AddSubscriptions();

            return services;
        }
    }
}