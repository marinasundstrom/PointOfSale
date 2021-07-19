using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Orders;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrders(this IServiceCollection services)
    {
        services.AddScoped<Mapper>();

        return services;
    }
}