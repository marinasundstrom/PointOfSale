using System;
using System.Collections.Generic;

using Payments.Contracts;

namespace Payments.Domain.Common
{
    public interface IHasDomainEvents
    {
        List<IDomainEvent> DomainEvents { get; set; }
    }
}