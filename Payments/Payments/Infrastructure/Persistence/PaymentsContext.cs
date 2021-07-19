using System;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Payments.Application.Common.Interfaces;
using Payments.Domain.Common;
using Payments.Domain.Entities;

namespace Payments.Infrastructure.Persistence
{
    public class PaymentsContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        private readonly IDomainEventService _domainEventService;

        public PaymentsContext(
            DbContextOptions<PaymentsContext> options,
            ICurrentUserService currentUserService,
            IDomainEventService domainEventService,
            IDateTime dateTime) : base(options)
        {
            _currentUserService = currentUserService;
            _domainEventService = domainEventService;
            _dateTime = dateTime;
        }


#nullable disable

        public DbSet<Payment> Payments { get; set; }

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

            await DispatchEvents();

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
            /*
            builder.Entity<Payment>(x => {
                x.Property(p => p.Data).HasConversion(jdoc => {
                    using(var stream = new MemoryStream())
                    {
                        Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
                        jdoc.WriteTo(writer);
                        writer.Flush();
                        return Encoding.UTF8.GetString(stream.ToArray());
                    }
                }, s => JsonDocument.Parse(s));
            });
            */

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

        private async Task DispatchEvents()
        {
            while (true)
            {
                var domainEventEntity = ChangeTracker.Entries<IHasDomainEvents>()
                    .Select(x => x.Entity.DomainEvents)
                    .SelectMany(x => x)
                    .Where(domainEvent => !domainEvent.IsPublished)
                    .FirstOrDefault();
                if (domainEventEntity == null) break;

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity);
            }
        }
    }
}