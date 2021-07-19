using System;

using Sales.Contracts;

namespace Sales.Domain.Common
{
    public interface IHasDomainEvents
    {
        List<IDomainEvent> DomainEvents { get; set; }
    }
}