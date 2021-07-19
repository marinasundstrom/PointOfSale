using System;
using System.Collections.Generic;

namespace Carts.Contracts
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public string? Tag { get; set; }
        public IEnumerable<CartItemDto> Items { get; set; } = null!;
        public IDictionary<double, CartTotalDto>? Totals { get; set; }
        public decimal SubTotal { get; set; }
        public double? VatRate { get; set; }
        public decimal Vat { get; set; }
        public IEnumerable<CartChargeDto> Charges { get; set; } = null!;
        public decimal? Charge { get; set; }
        public IEnumerable<CartDiscountDto> Discounts { get; set; } = null!;
        public decimal? Discount { get; set; }
        public decimal? Rounding { get; set; }
        public decimal Total { get; set; }
    }

    public class CartItemDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = null!;
        public string? ItemId { get; set; }
        public UnitDto? Unit { get; set; }
        public decimal Price { get; set; }
        public double VatRate { get; set; }
        public double Quantity { get; set; }
        public IEnumerable<CartChargeDto> Charges { get; set; } = null!;
        public decimal? Charge { get; set; }
        public IEnumerable<CartDiscountDto> Discounts { get; set; } = null!;
        public decimal? Discount { get; set; }
        public decimal Total { get; set; }
    }

    public class UnitDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Code { get; set; }
    }

    public class CartTotalDto
    {
        public CartTotalDto(decimal subTotal, decimal vat, decimal total)
        {
            this.SubTotal = subTotal;
            this.Vat = vat;
            this.Total = total;
        }

        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public decimal Total { get; set; }
    }

    public class CartTotalsDto
    {
        public IDictionary<double, CartTotalDto>? Totals { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public IEnumerable<CartDiscountDto>? Discounts { get; set; }
        public decimal? Rounding { get; set; }
        public decimal Total { get; set; }
    }

    public class CartItemProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class CartChargeDto
    {
        public Guid Id { get; set; }
        public int? Quantity { get; set; }
        public int? Limit { get; set; }
        public decimal? Amount { get; set; }
        public double? Percent { get; set; }
        public decimal Total { get; set; }
        public string Description { get; set; } = null!;
        public Guid? ChargeId { get; set; }
    }

    public class CartDiscountDto
    {
        public Guid Id { get; set; }
        public int? Quantity { get; set; }
        public int? Limit { get; set; }
        public decimal? Amount { get; set; }
        public double? Percent { get; set; }
        public decimal Total { get; set; }
        public string Description { get; set; } = null!;
        public Guid? DiscountId { get; set; }
    }
}