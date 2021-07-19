using System;

namespace Payments.Contracts
{
    public interface IDomainEvent
    {
        bool IsPublished { get; set; }
    }

    public abstract class DomainEvent : IDomainEvent
    {
        public bool IsPublished { get; set; }
    }

    public class PaymentConfirmedEvent : DomainEvent
    {
        public string OrderRef { get; set; } = null!;
    }

    public class PaymentCancelledEvent : DomainEvent
    {
        public string OrderRef { get; set; } = null!;
    }
}