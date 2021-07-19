using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sales.Application.Common.Interfaces;
using Sales.Infrastructure.Persistence;
using Sales.Infrastructure.Services;
using Sales.Services;

namespace Sales.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistence(configuration);

            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IDateTime, DateTimeService>();

            services.AddSingleton<IDomainEventService, DomainEventService>();

            return services;
        }
    }
}