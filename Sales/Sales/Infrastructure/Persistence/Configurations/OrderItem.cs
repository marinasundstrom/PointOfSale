
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sales.Domain.Entities;

namespace Sales.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Ignore(e => e.DomainEvents);

            builder.HasQueryFilter(e => e.Deleted == null);
        }
    }
}