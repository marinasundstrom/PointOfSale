using System;

namespace Checkout.Controllers
{
    public class CheckoutDto
    {
        public Guid CartId { get; set; }

        public int? CustomerNo { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public string? Payer { get; set; }

        public decimal? Amount { get; set; }

        public string? Signature { get; set; }
    }
}