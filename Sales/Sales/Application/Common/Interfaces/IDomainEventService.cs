using System;

using Sales.Contracts;
using Sales.Domain.Common;

namespace Sales.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(IDomainEvent domainEvent);
    }
}