using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BillingContext>(
            (sp, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("db", "Billing"),
                    b => b.MigrationsAssembly(typeof(BillingContext).Assembly.FullName));
#if DEBUG
                options.EnableSensitiveDataLogging();
#endif
            });

        //services.AddScoped<IBillingContext>(sp => sp.GetRequiredService<BillingContext>());

        return services;
    }
}