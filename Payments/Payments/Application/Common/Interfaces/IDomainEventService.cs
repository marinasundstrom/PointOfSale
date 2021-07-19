using System;

using Payments.Contracts;
using Payments.Domain.Common;

namespace Payments.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(IDomainEvent domainEvent);
    }
}