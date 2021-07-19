using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Catalog;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCatalog(this IServiceCollection services)
    {
        services.AddScoped<Mapper>();

        return services;
    }
}