using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Carts;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCarts(this IServiceCollection services)
    {
        services.AddScoped<Mapper>();

        return services;
    }
}