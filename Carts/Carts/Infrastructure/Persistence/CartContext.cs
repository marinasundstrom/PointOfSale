using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Carts.Application.Common.Interfaces;
using Carts.Domain.Entities;
using Carts.Domain.Entities.Common;

using Microsoft.EntityFrameworkCore;

namespace Carts.Infrastructure.Persistence
{
    public class CartContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        //private readonly IDomainEventService _domainEventService;

        public CartContext(
            DbContextOptions<CartContext> options,
            ICurrentUserService currentUserService,
            //IDomainEventService domainEventService,
            IDateTime dateTime) : base(options)
        {
            _currentUserService = currentUserService;
            //_domainEventService = domainEventService;
            _dateTime = dateTime;
        }

#nullable disable

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CartDiscount> CartDiscounts { get; set; }
        public DbSet<CartCharge> CartCharges { get; set; }

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