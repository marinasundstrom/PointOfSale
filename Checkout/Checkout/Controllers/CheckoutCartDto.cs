using System;
using System.Collections.Generic;

using Carts.Client;

namespace Checkout.Controllers
{
    public class CheckoutCartDto
    {
        public Guid Id { get; set; }
        public string? Tag { get; set; }
        public CheckoutPersonDto? Customer { get; set; }
        public IEnumerable<CheckoutCartItemDto> Items { get; set; } = null!;
        public IDictionary<string, CheckoutVatSummaryDto>? Vat { get; set; }
        public decimal? Rounding { get; set; }
        public decimal Total { get; set; }
    }

    public class CheckoutPersonDto
    {
        public Guid Id { get; set; }

        public int CustomerNo { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Ssn { get; set; } = null!;
    }

    public class CheckoutCartItemDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = null!;
        public string? ItemId { get; set; }
        public UnitDto? Unit { get; set; }
        public decimal Price { get; set; }
        public double VatRate { get; set; }
        public double Quantity { get; set; }
        public decimal Total { get; set; }
    }

    public class CheckoutVatSummaryDto
    {
        public CheckoutVatSummaryDto(decimal subTotal, decimal vat, decimal total)
        {
            this.SubTotal = subTotal;
            this.Vat = vat;
            this.Total = total;
        }

        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public decimal Total { get; set; }
    }
}