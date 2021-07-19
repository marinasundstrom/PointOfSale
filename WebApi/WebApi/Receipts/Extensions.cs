using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Receipts;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReceipts(this IServiceCollection services)
    {
        services.AddScoped<Mapper>();

        return services;
    }
}