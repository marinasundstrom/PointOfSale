using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Sales.Domain.Entities;

namespace Sales.Infrastructure.Persistence
{
    public static class SalesContextSeed
    {
        public static async Task SeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<SalesContext>();

            //await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            if (!context.OrderStatuses.Any())
            {
                context.OrderStatuses.Add(new OrderStatus("draft", "Draft"));
                context.OrderStatuses.Add(new OrderStatus("placed", "Placed"));
                context.OrderStatuses.Add(new OrderStatus("cancelled", "Cancelled"));
                context.OrderStatuses.Add(new OrderStatus("paid", "Paid"));
            }

            if (!context.CustomFieldDefinitions.Any())
            {
                context.CustomFieldDefinitions.Add(new CustomFieldDefinition
                {
                    Id = "serialNo",
                    Name = "Serial number",
                    Type = CustomFieldType.String
                });

                context.CustomFieldDefinitions.Add(new CustomFieldDefinition
                {
                    Id = "regularPrice",
                    Name = "Regular price",
                    Type = CustomFieldType.String
                });

                context.CustomFieldDefinitions.Add(new CustomFieldDefinition
                {
                    Id = "objectId",
                    Name = "ObjectId",
                    Type = CustomFieldType.String
                });

                context.CustomFieldDefinitions.Add(new CustomFieldDefinition
                {
                    Id = "orderNo",
                    Name = "OrderNo",
                    Type = CustomFieldType.Integer
                });

                context.CustomFieldDefinitions.Add(new CustomFieldDefinition
                {
                    Id = "customerNo",
                    Name = "CustomerNo",
                    Type = CustomFieldType.Integer
                });
            }

            await context.SaveChangesAsync();
        }
    }
}