
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sales.Domain.Entities;

namespace Sales.Infrastructure.Persistence.Configurations
{
    public class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> builder)
        {
            builder.Ignore(e => e.DomainEvents);

            builder.HasQueryFilter(e => e.Deleted == null);
        }
    }
}