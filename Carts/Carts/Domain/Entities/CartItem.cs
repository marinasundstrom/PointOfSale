using System;
using System.Collections.Generic;

using Carts.Domain.Entities.Common;

using OrderPriceCalculator;

namespace Carts.Domain.Entities
{
    public class CartItem : AuditableEntity, IOrderItem
    {
        public Guid Id { get; set; }

        public Cart Cart { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string ItemId { get; set; } = null!;

        public string? Unit { get; set; }

        public decimal Price { get; set; }

        public double VatRate { get; set; }

        public double Quantity { get; set; }

        public CartItem UpdateQuantity(double quantity)
        {
            Quantity = quantity;

            return this;
        }

        public ICollection<CartCharge> Charges { get; set; } = new List<CartCharge>();

        public decimal? Charge { get; set; }

        public ICollection<CartDiscount> Discounts { get; set; } = new List<CartDiscount>();

        public decimal? Discount { get; set; }

        IEnumerable<ICharge> IHasCharges.Charges => Charges;
        IEnumerable<IDiscount> IHasDiscounts.Discounts => Discounts;
    }
}