using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Hal;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHal(this IServiceCollection services)
    {
        services.AddScoped<Mapper>();

        return services;
    }
}