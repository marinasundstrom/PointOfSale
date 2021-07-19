using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Warehouse.Infrastructure.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<WarehouseContext>(
                (sp, options) =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("db", "Warehouse"),
                        b => b.MigrationsAssembly(typeof(WarehouseContext).Assembly.FullName));
#if DEBUG
                    options.EnableSensitiveDataLogging();
#endif
                });

            //services.AddScoped<IWarehouseContext>(sp => sp.GetRequiredService<WarehouseContext>());

            return services;
        }
    }
}