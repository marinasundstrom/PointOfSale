using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Payments.Domain.Entities;

namespace Payments.Infrastructure.Persistence
{
    public static class PaymentsContextSeed
    {
        public static async Task SeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            //await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await context.SaveChangesAsync();
        }
    }
}