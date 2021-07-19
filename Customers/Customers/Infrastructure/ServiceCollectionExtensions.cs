using Customers.Application.Common.Interfaces;
using Customers.Infrastructure.Persistence;
using Customers.Infrastructure.Services;
using Customers.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customers.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistence(configuration);

            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IDateTime, DateTimeService>();

            return services;
        }
    }
}