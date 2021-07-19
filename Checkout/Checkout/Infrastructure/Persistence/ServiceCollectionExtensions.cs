using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Checkout.Infrastructure.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CheckoutContext>(
                (sp, options) =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("db", "Checkout"),
                        b => b.MigrationsAssembly(typeof(CheckoutContext).Assembly.FullName));
#if DEBUG
                    options.EnableSensitiveDataLogging();
#endif
                });

            //services.AddScoped<ICheckoutContext>(sp => sp.GetRequiredService<CheckoutContext>());

            return services;
        }
    }
}