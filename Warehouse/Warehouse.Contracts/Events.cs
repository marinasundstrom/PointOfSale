using System.Collections.Generic;

namespace Warehouse.Contracts
{
    public interface IDomainEvent
    {
        bool IsPublished { get; set; }
    }

    public abstract class DomainEvent : IDomainEvent
    {
        public bool IsPublished { get; set; }
    }

    public class WarehouseItemQuantityOnHandUpdatedEvent : DomainEvent
    {
        public string ItemId { get; set; } = null!;
    }

    public class WarehouseItemQuantityAvailableUpdatedEvent : DomainEvent
    {
        public string ItemId { get; set; } = null!;
    }
    public class WarehouseItemQuantityReservedUpdatedEvent : DomainEvent
    {
        public string ItemId { get; set; } = null!;
    }
}