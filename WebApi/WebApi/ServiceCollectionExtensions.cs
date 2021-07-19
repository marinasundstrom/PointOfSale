
using MassTransit;

using Microsoft.Extensions.DependencyInjection;

using WebApi.Carts;
using WebApi.Hal;
using WebApi.Orders;
using WebApi.OrderStatuses;
using WebApi.Receipts;
using WebApi.Catalog;
using WebApi.Customers;

namespace WebApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddHal();

        services.AddCarts();
        services.AddOrders();
        services.AddOrderStatuses();
        services.AddReceipts();
        services.AddCatalog();
        services.AddCustomers();

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumers(typeof(ServiceCollectionExtensions).Assembly);
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