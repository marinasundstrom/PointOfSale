using System;
using System.Threading.Tasks;

using Catalog.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure.Persistence
{
    public static class CatalogContextSeed
    {
        public static async Task SeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();

            //await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            if (context.VatCodes.Any())
                return;

            context.VatCodes.Add(new VatCode()
            {
                Name = "Ingen moms",
                VatRate = 0
            });

            context.VatCodes.Add(new VatCode()
            {
                Name = "6%",
                VatRate = 0.06
            });

            context.VatCodes.Add(new VatCode()
            {
                Name = "12%",
                VatRate = 0.12
            });

            context.VatCodes.Add(new VatCode()
            {
                Name = "25%",
                VatRate = 0.25
            });

            var piece = new Unit()
            {
                Name = "Piece",
                Code = "pc",
            };

            context.Units.Add(piece);

            await context.SaveChangesAsync();

            if (!context.CustomFieldDefinitions.Any())
            {
                context.CustomFieldDefinitions.Add(new CustomFieldDefinition
                {
                    Id = "compareAtPrice",
                    Name = "Compare At Price",
                    Type = CustomFieldType.Integer
                });

                await context.SaveChangesAsync();
            }

            context.Items.Add(new CatalogItem()
            {
                Id = Guid.NewGuid(),
                ItemId = "1581914908",
                Name = "Kaffe",
                Code = "263456789129",
                Unit = piece,
                Price = 20m,
                VatIncluded = true,
                VatCode = 3
            });

            context.Items.Add(new CatalogItem()
            {
                Id = Guid.NewGuid(),
                ItemId = "1299307470",
                Name = "Chokladboll",
                Code = "833476781243",
                Unit = piece,
                Price = 15m,
                VatIncluded = true,
                VatCode = 3
            });

            context.Items.Add(new CatalogItem()
            {
                Id = Guid.NewGuid(),
                ItemId = "4038059373",
                Name = "Glass",
                Code = "893474787237",
                Unit = piece,
                Price = 20m,
                VatIncluded = true,
                VatCode = 3
            });

            context.Items.Add(new CatalogItem()
            {
                Id = Guid.NewGuid(),
                ItemId = "9152567409",
                Name = "Skjorta",
                Code = "49527431793",
                Unit = piece,
                Price = 150m,
                VatIncluded = true,
                VatCode = 4
            });

            context.Items.Add(new CatalogItem()
            {
                Id = Guid.NewGuid(),
                ItemId = "1519869956",
                Name = "Hallonsaft",
                Code = "735674768213",
                Unit = piece,
                Price = 10m,
                VatIncluded = true,
                VatCode = 3
            });

            context.Items.Add(new CatalogItem()
            {
                Id = Guid.NewGuid(),
                ItemId = "7389942365",
                Name = "Kanelbulle",
                Code = "675844768342",
                Unit = piece,
                Price = 15m,
                VatIncluded = true,
                VatCode = 3
            });

            var item = new CatalogItem()
            {
                Id = Guid.NewGuid(),
                ItemId = "8944331109",
                Name = "Trikåbyxor",
                Code = "263456789129",
                Unit = piece,
                Price = 130m,
                VatIncluded = true,
                VatCode = 4
            };

            item.CustomFields.Add(new CustomField() {
                CustomFieldId = "compareAtPrice",
                Value = "170"
            });

            context.Items.Add(item);

            context.Items.Add(new CatalogItem()
            {
                Id = Guid.NewGuid(),
                ItemId = "671987986",
                Name = "Kebabtallrik",
                Code = "745374756239",
                Unit = piece,
                Price = 79m,
                VatIncluded = true,
                VatCode = 3
            });

            context.Items.Add(new CatalogItem()
            {
                Id = Guid.NewGuid(),
                ItemId = "2535863758",
                Name = "Coca Cola Zero 0.5l",
                Code = "739272914213",
                Unit = piece,
                Price = 12.90m,
                VatIncluded = true,
                VatCode = 3,
                Charges = new List<CatalogItemCharge>() {
                    new CatalogItemCharge() {
                        Id = Guid.NewGuid(),
                        Name = "Pant",
                        Amount = 1
                    }
                }
            });

            await context.SaveChangesAsync();
        }
    }
}