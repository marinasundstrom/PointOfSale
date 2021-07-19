using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CatalogContext>(
                (sp, options) =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("db", "Catalog"),
                        b => b.MigrationsAssembly(typeof(CatalogContext).Assembly.FullName));
#if DEBUG
                    options.EnableSensitiveDataLogging();
#endif
                });

            //services.AddScoped<ICatalogContext>(sp => sp.GetRequiredService<CatalogContext>());

            return services;
        }
    }
}