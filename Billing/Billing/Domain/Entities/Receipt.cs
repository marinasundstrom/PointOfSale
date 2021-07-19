using System;
using System.Collections.Generic;
using System.Linq;

using Billing.Domain.Entities.Common;
using Billing.Domain.Enums;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

namespace Billing.Domain.Entities;

public class Receipt : AuditableEntity, IOrder2WithTotals, IOrder2WithTotalsInternals
{
    public Guid Id { get; set; }

    public int ReceiptNo { get; set; }

    public DateTime Date { get; set; }

    public ReceiptType Type { get; set; } = ReceiptType.Receipt;

    public ReceiptStatus Status { get; set; } = ReceiptStatus.Draft;

    public ICollection<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();

    public ICollection<ReceiptTotals> Totals { get; set; } = new List<ReceiptTotals>();

    public decimal? SubTotal { get; set; }

    public decimal? Vat { get; set; }

    public double? VatRate { get; set; }

    public List<ReceiptCharge> Charges { get; set; } = new List<ReceiptCharge>();

    public decimal? Charge { get; set; }

    public ICollection<ReceiptDiscount> Discounts { get; set; } = new List<ReceiptDiscount>();

    public decimal? Discount { get; set; }

    public decimal? Rounding { get; set; }

    public decimal Total { get; set; }

    public PaymentDetails? Payment { get; set; }

    public string? Signature { get; set; }

    public List<CustomField> CustomFields { get; set; } = new List<CustomField>();

    IEnumerable<IOrderItem> IOrder.Items => Items;
    IEnumerable<IOrderItem2> IOrder2.Items => Items;

    IEnumerable<IOrderTotals> IOrder2WithTotals.Totals => Totals;

    IEnumerable<ICharge> IHasCharges.Charges => Charges;
    IEnumerable<IChargeWithTotal> IHasChargesWithTotal.Charges => Charges;

    IEnumerable<IDiscount> IHasDiscounts.Discounts => Discounts;
    IEnumerable<IDiscountWithTotal> IHasDiscountsWithTotal.Discounts => Discounts;

    void IOrder2WithTotalsInternals.AddTotals(IOrderTotals ReceiptTotals)
    {
        Totals.Add((ReceiptTotals)ReceiptTotals);
    }

    void IOrder2WithTotalsInternals.RemoveTotals(IOrderTotals ReceiptTotals)
    {
        Totals.Remove((ReceiptTotals)ReceiptTotals);
    }

    void IOrder2WithTotalsInternals.ClearTotals()
    {
        Totals.Clear();
    }

    IOrderTotals IOrder2WithTotalsInternals.CreateTotals(double vatRate, decimal subTotal, decimal vat, decimal total)
    {
        return new ReceiptTotals()
        {
            VatRate = vatRate,
            SubTotal = subTotal,
            Vat = vat,
            Total = total
        };
    }
}

public class ReceiptTotals : IOrderTotals
{
    public ReceiptTotals() { }

    public ReceiptTotals(double vatRate, decimal subTotal, decimal vat, decimal total)
    {
        Id = Guid.NewGuid();
        VatRate = vatRate;
        SubTotal = subTotal;
        Vat = vat;
        Total = total;
    }

    public Guid Id { get; set; }

    public Receipt Receipt { get; set; } = null!;

    public double VatRate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Vat { get; set; }

    public decimal Total { get; set; }
}

[Owned]
public class PaymentDetails
{
    public decimal? Paid { get; set; }

    public decimal? Returned { get; set; }
}

public class ReceiptItem : AuditableEntity, IOrderItem2
{
    public Guid Id { get; set; }

    public Receipt Receipt { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? ItemId { get; set; }

    public string? Unit { get; set; }

    public decimal Price { get; set; }

    public double VatRate { get; set; }

    public double Quantity { get; set; }

    public ReceiptItem UpdateQuantity(double quantity)
    {
        Quantity = quantity;

        return this;
    }

    public List<ReceiptCharge> Charges { get; set; } = new List<ReceiptCharge>();

    public decimal? Charge { get; set; }

    public ICollection<ReceiptDiscount> Discounts { get; set; } = new List<ReceiptDiscount>();

    public decimal? Discount { get; set; }

    public decimal Vat { get; set; }

    public decimal Total { get; set; }

    public void UpdateTotal()
    {
        Discount = this.Discount();
        Total = Price * (decimal)Quantity + Discount.GetValueOrDefault();
    }

    public List<CustomField> CustomFields { get; set; } = new List<CustomField>();

    IEnumerable<ICharge> IHasCharges.Charges => Charges;
    IEnumerable<IChargeWithTotal> IHasChargesWithTotal.Charges => Charges;

    IEnumerable<IDiscount> IHasDiscounts.Discounts => Discounts;
    IEnumerable<IDiscountWithTotal> IHasDiscountsWithTotal.Discounts => Discounts;
}

public class ReceiptCharge : IChargeWithTotal
{
    public Guid Id { get; set; }

    public Receipt? Receipt { get; set; } = null!;

    public ReceiptItem? ReceiptItem { get; set; }

    public string Description { get; set; } = null!;

    public Guid? ChargeId { get; set; }

    public int? Quantity { get; set; }

    public int? Limit { get; set; }

    public decimal? Amount { get; set; }

    public double? Percent { get; set; }

    public decimal Total { get; set; }
}

public class ReceiptDiscount : AuditableEntity, IDiscountWithTotal
{
    public Guid Id { get; set; }

    public Receipt? Receipt { get; set; } = null!;

    public ReceiptItem? ReceiptItem { get; set; }

    public int? Quantity { get; set; }

    public int? Limit { get; set; }

    public decimal? Amount { get; set; }

    public double? Percent { get; set; }

    public decimal Total { get; set; }

    public string Description { get; set; } = null!;

    public Guid? DiscountId { get; set; }
}