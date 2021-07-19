using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marketing.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MarketingContext>(
            (sp, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("db", "Marketing"),
                    b => b.MigrationsAssembly(typeof(MarketingContext).Assembly.FullName));
#if DEBUG
                options.EnableSensitiveDataLogging();
#endif
            });

        //services.AddScoped<IMarketingContext>(sp => sp.GetRequiredService<MarketingContext>());

        return services;
    }
}