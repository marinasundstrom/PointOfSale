using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Customers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomers(this IServiceCollection services)
    {
        services.AddScoped<Mapper>();

        return services;
    }
}