using Carts.Application.Common.Interfaces;
using Carts.Infrastructure.Persistence;
using Carts.Infrastructure.Services;
using Carts.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Carts.Infrastructure
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