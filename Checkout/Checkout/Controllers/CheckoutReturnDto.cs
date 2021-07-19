using System.Collections.Generic;

namespace Checkout.Controllers
{
    public class CheckoutReturnDto
    {
        public int ReceiptNo { get; set; }

        public IEnumerable<CheckoutReturnItemDto> Items { get; set; } = null!;

        public string? Signature { get; set; }
    }
}