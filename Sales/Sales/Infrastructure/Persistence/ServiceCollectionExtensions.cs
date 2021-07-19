using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sales.Infrastructure.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SalesContext>(
                (sp, options) =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("db", "Sales"),
                        b => b.MigrationsAssembly(typeof(SalesContext).Assembly.FullName));
#if DEBUG
                    options.EnableSensitiveDataLogging();
#endif
                });

            //services.AddScoped<ISalesContext>(sp => sp.GetRequiredService<SalesContext>());

            return services;
        }
    }
}