using Microsoft.Extensions.DependencyInjection;

namespace WebApi.OrderStatuses;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderStatuses(this IServiceCollection services)
    {
        services.AddScoped<Mapper>();

        return services;
    }
}