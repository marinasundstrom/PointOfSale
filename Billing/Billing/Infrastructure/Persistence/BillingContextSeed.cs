using Billing.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

namespace Billing.Infrastructure.Persistence;

public static class BillingContextSeed
{
    public static async Task SeedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<BillingContext>();

        //await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        if (context.CustomFieldDefinitions.Any())
            return;

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

        await context.SaveChangesAsync();
    }
}