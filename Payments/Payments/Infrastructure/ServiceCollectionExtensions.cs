using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Payments.Application.Common.Interfaces;
using Payments.Infrastructure.Persistence;
using Payments.Infrastructure.Services;
using Payments.Services;

namespace Payments.Infrastructure
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