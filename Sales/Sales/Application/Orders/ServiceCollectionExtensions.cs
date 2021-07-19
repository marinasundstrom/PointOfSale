using Microsoft.Extensions.DependencyInjection;

namespace Sales.Application.Orders
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOrders(this IServiceCollection services)
        {
            services.AddScoped<OrderFactory>();

            return services;
        }
    }
}