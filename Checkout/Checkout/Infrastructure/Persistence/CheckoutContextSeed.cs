using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Checkout.Infrastructure.Persistence
{
    public static class CheckoutContextSeed
    {
        public static async Task SeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<CheckoutContext>();

            //await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            /*
            context.Receipts.Add(new Receipt()
            {
                ReceiptNo = 1,
                Date = DateTime.Now,
                Status = Domain.Enums.ReceiptStatus.Draft
            });

            await context.SaveChangesAsync(); */
        }
    }
}