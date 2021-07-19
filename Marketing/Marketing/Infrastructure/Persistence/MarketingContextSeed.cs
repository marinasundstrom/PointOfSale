using System;
using System.Threading.Tasks;

using Marketing.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

namespace Marketing.Infrastructure.Persistence;

public static class MarketingContextSeed
{
    public static async Task SeedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<MarketingContext>();

        //await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        if (context.Discounts.Any())
            return;

        context.Discounts.Add(new Discount()
        {
            Id = Guid.NewGuid(),
            Name = "3 för 2",
            Description = "",

            ItemId = "9152567409",
            Quantity = 3,

            Amount = -150m
        });

        context.Discounts.Add(new Discount()
        {
            Id = Guid.NewGuid(),
            Name = "Fikaerbjudande - 10 kr rabatt vid köp av kaffe",
            Description = "",

            ItemId = "7389942365",
            Quantity = 1,

            OtherItemId = "1581914908",

            Amount = -10m
        });

        await context.SaveChangesAsync();
    }
}