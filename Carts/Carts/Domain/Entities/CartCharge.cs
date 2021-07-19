using System;
using System.Collections.Generic;

using Carts.Domain.Entities.Common;

using OrderPriceCalculator;

namespace Carts.Domain.Entities
{
    public class CartCharge : AuditableEntity, ICharge
    {
        public Guid Id { get; set; }

        public Cart? Cart { get; set; } = null!;

        public CartItem? CartItem { get; set; }

        public int? Quantity { get; set; }

        public int? Limit { get; set; }

        public decimal? Amount { get; set; }

        public double? Percent { get; set; }

        public decimal Total { get; set; }

        public string Description { get; set; } = null!;

        public Guid? ChargeId { get; set; } = null!;
    }
}