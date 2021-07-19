using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Checkout.Application.Common.Interfaces;
using Checkout.Domain.Entities.Common;

using Microsoft.EntityFrameworkCore;

namespace Checkout.Infrastructure.Persistence
{
    public class CheckoutContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        //private readonly IDomainEventService _domainEventService;

        public CheckoutContext(
            DbContextOptions<CheckoutContext> options,
            ICurrentUserService currentUserService,
            //IDomainEventService domainEventService,
            IDateTime dateTime) : base(options)
        {
            _currentUserService = currentUserService;
            //_domainEventService = domainEventService;
            _dateTime = dateTime;
        }

#nullable disable

        /*
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptItem> ReceiptItems { get; set; }
        public DbSet<ReceiptVat> ReceiptVats { get; set; }
        */

#nullable restore

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        entry.Entity.Created = _dateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        entry.Entity.LastModified = _dateTime.Now;
                        break;

                    case EntityState.Deleted:
                        if (entry.Entity is ISoftDelete softDelete)
                        {
                            softDelete.DeletedBy = _currentUserService.UserId;
                            softDelete.Deleted = _dateTime.Now;

                            entry.State = EntityState.Modified;
                        }
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            //await DispatchEvents();

            return result;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            optionsBuilder.LogTo(Console.WriteLine);
#endif
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }
    }
}