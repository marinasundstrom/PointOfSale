using System;
using System.ComponentModel.DataAnnotations.Schema;

using Warehouse.Contracts;
using Warehouse.Domain.Common;

namespace Warehouse.Domain.Entities
{
    public class WarehouseItem : AuditableEntity, IHasDomainEvents, ISoftDelete
    {
        public Guid Id { get; set; }

        public string ItemId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Sku { get; set; }

        public int? QuantityOnHand { get; set; }
        public int? QuantityPicked { get; set; } // Do I need now ?
        public int? QuantityReserved { get; set; }
        public int QuantityAvailable => QuantityOnHand.GetValueOrDefault() - QuantityReserved.GetValueOrDefault();

        public string? Code { get; set; }

        public List<IDomainEvent> DomainEvents { get; set; } = new List<IDomainEvent>();

        public DateTime? Deleted { get; set; }

        public string? DeletedBy { get; set; }

        public WarehouseItem AdjustQuantityOnHand(int quantity)
        {
            QuantityOnHand = quantity;

            DomainEvents.Add(new WarehouseItemQuantityOnHandUpdatedEvent()
            {
                ItemId = ItemId
            });

            DomainEvents.Add(new WarehouseItemQuantityAvailableUpdatedEvent()
            {
                ItemId = ItemId
            });

            return this;
        }

        public WarehouseItem ReserveQuantity(int quantity)
        {
            QuantityReserved = QuantityReserved.GetValueOrDefault() + quantity;

            DomainEvents.Add(new WarehouseItemQuantityReservedUpdatedEvent()
            {
                ItemId = ItemId
            });

            DomainEvents.Add(new WarehouseItemQuantityAvailableUpdatedEvent()
            {
                ItemId = ItemId
            });

            return this;
        }
    }
}