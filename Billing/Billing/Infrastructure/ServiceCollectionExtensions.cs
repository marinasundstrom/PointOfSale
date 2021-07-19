using Billing.Application.Common.Interfaces;
using Billing.Infrastructure.Persistence;
using Billing.Infrastructure.Services;
using Billing.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Infrastructure;

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