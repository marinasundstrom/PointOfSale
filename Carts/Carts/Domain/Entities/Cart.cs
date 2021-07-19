using System;
using System.Collections.Generic;

using Carts.Domain.Entities.Common;

using OrderPriceCalculator;

namespace Carts.Domain.Entities
{
    public class Cart : AuditableEntity, IOrder
    {
        public Guid Id { get; set; }

        public string? Tag { get; set; }

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

        public ICollection<CartCharge> Charges { get; set; } = new List<CartCharge>();

        public decimal? Charge { get; set; }

        public ICollection<CartDiscount> Discounts { get; set; } = new List<CartDiscount>();

        public decimal? Discount { get; set; }

        IEnumerable<IOrderItem> IOrder.Items => Items;

        IEnumerable<ICharge> IHasCharges.Charges => Charges;
        IEnumerable<IDiscount> IHasDiscounts.Discounts => Discounts;
    }
}