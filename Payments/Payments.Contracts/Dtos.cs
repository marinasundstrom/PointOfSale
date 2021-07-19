using System;

namespace Payments.Contracts
{
    public class PaymentDto
    {
        public Guid Id { get; set; }

        public PaymentStatus Status { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public string Data { get; set; } = null!;

        public decimal AmountRequested { get; set; }

        public decimal? AmountConfirmed { get; set; }

        public DateTime Due { get; set; }

        public string OrderRef { get; set; } = null!;
    }

    public enum PaymentStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
}