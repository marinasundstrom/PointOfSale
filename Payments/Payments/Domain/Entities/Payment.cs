using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

using Payments.Contracts;

using Payments.Domain.Common;
using Payments.Domain.Enums;

namespace Payments.Domain.Entities
{
    public class Payment : AuditableEntity, ISoftDelete, IHasDomainEvents
    {
        public Guid Id { get; set; }

        public Domain.Enums.PaymentStatus Status { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public string Data { get; set; } = null!;

        public decimal AmountRequested { get; set; }

        public decimal? AmountConfirmed { get; set; }

        public DateTime Due { get; set; }

        public string OrderRef { get; set; } = null!;

        public DateTime? Deleted { get; set; }

        public string? DeletedBy { get; set; }

        public List<IDomainEvent> DomainEvents { get; set; } = new List<IDomainEvent>();
    }
}