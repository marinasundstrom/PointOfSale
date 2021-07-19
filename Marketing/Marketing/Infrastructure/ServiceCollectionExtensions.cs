using Marketing.Application.Common.Interfaces;
using Marketing.Infrastructure.Persistence;
using Marketing.Infrastructure.Services;
using Marketing.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marketing.Infrastructure;

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