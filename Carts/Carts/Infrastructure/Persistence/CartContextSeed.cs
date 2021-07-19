using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Carts.Infrastructure.Persistence
{
    public static class CartContextSeed
    {
        public static async Task SeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<CartContext>();

            //await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            if (context.Carts.Any(x => x.Tag == null))
            {
                return;
            }

            context.Carts.Add(new Domain.Entities.Cart()
            {
                Id = Guid.NewGuid(),
                Tag = null
            });

            await context.SaveChangesAsync();
        }
    }
}