using System;

namespace Carts
{
    public class AddDiscountDto
    {
        public decimal? Amount { get; set; }

        public double? Percent { get; set; }

        public decimal? Total { get; set; }

        public int? Quantity { get; set; }

        public string? Description { get; set; }

        public Guid? DiscountId { get; set; }
    }
}