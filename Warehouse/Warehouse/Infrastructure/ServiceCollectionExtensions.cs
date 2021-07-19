using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Warehouse.Application.Common.Interfaces;
using Warehouse.Infrastructure.Persistence;
using Warehouse.Infrastructure.Services;
using Warehouse.Services;

namespace Warehouse.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistence(configuration);

            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IDateTime, DateTimeService>();

            services.AddScoped<IDomainEventService, DomainEventService>();

            return services;
        }
    }
}