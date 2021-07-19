using System;

using Warehouse.Contracts;

namespace Warehouse.Domain.Common
{
    public interface IHasDomainEvents
    {
        List<IDomainEvent> DomainEvents { get; set; }
    }
}