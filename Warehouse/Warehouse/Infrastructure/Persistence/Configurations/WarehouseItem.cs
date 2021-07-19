
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Warehouse.Domain.Entities;

namespace Warehouse.Infrastructure.Persistence.Configurations
{
    public class WarehouseItemConfiguration : IEntityTypeConfiguration<WarehouseItem>
    {
        public void Configure(EntityTypeBuilder<WarehouseItem> builder)
        {
            builder.Ignore(e => e.DomainEvents);

            builder.HasQueryFilter(e => e.Deleted == null);
        }
    }
}