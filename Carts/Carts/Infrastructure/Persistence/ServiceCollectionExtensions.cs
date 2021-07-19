using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Carts.Infrastructure.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CartContext>(
                (sp, options) =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("db", "Carts"),
                        b => b.MigrationsAssembly(typeof(CartContext).Assembly.FullName));
#if DEBUG
                    options.EnableSensitiveDataLogging();
#endif
                });

            //services.AddScoped<ICartContext>(sp => sp.GetRequiredService<CartContext>());

            return services;
        }
    }
}