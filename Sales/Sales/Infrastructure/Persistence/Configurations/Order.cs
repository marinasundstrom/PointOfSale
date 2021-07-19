
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sales.Domain.Entities;

namespace Sales.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Ignore(e => e.DomainEvents);

            builder.HasQueryFilter(e => e.Deleted == null);
        }
    }
}