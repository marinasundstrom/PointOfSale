using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Warehouse.Domain.Entities;

namespace Warehouse.Infrastructure.Persistence
{
    public static class WarehouseContextSeed
    {
        public static async Task SeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<WarehouseContext>();

            await SeedAsync(context);
        }

        public static async Task SeedAsync(this WarehouseContext context)
        {
            //await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            if (context.Items.Any())
                return;

            /*
                        context.Items.Add(new WarehouseItem()
                        {
                            Id = Guid.NewGuid(),
                            ItemId = "1581914908",
                            Name = "Kaffe",
                            Code = "263456789129"
                        });
            */

            context.Items.Add(new WarehouseItem()
            {
                Id = Guid.NewGuid(),
                ItemId = "1299307470",
                Name = "Chokladboll",
                Code = "833476781243",
                QuantityOnHand = 10
            }
            .AdjustQuantityOnHand(10));

            /*
                        context.Items.Add(new WarehouseItem()
                        {
                            Id = Guid.NewGuid(),
                            ItemId = "4038059373",
                            Name = "Glass",
                            Code = "893474787237"
                        });
            */

            context.Items.Add(new WarehouseItem()
            {
                Id = Guid.NewGuid(),
                ItemId = "9152567409",
                Name = "Skjorta",
                Code = "49527431793",
            }
            .AdjustQuantityOnHand(90)
            .ReserveQuantity(2));

            /*
                        context.Items.Add(new WarehouseItem()
                        {
                            Id = Guid.NewGuid(),
                            ItemId = "1519869956",
                            Name = "Hallonsaft",
                            Code = "735674768213"
                        });
            */

            context.Items.Add(new WarehouseItem()
            {
                Id = Guid.NewGuid(),
                ItemId = "7389942365",
                Name = "Kanelbulle",
                Code = "675844768342",
            }
            .AdjustQuantityOnHand(10));

            context.Items.Add(new WarehouseItem()
            {
                Id = Guid.NewGuid(),
                ItemId = "8944331109",
                Name = "Trikåbyxor",
                Code = "263456789129",
            }
            .AdjustQuantityOnHand(26));

            /*
                        context.Items.Add(new WarehouseItem()
                        {
                            Id = Guid.NewGuid(),
                            ItemId = "671987986",
                            Name = "Kebabtallrik",
                            Code = "745374756239"
                        });
            */

            context.Items.Add(new WarehouseItem()
            {
                Id = Guid.NewGuid(),
                ItemId = "2535863758",
                Name = "Coca Cola Zero 0.5l",
                Code = "739272914213",
            }
            .AdjustQuantityOnHand(100));

            await context.SaveChangesAsync();
        }
    }
}