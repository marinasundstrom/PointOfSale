using System;

namespace Checkout.Controllers
{
    public class CheckoutReturnItemDto
    {
        public string ItemId { get; set; } = null!;

        public double Quantity { get; set; }
    }
}