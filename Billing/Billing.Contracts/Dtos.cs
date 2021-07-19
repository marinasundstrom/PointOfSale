namespace Billing.Contracts;

public class CreateReceiptDto
{
    public ReceiptType ReceiptType { get; set; } = ReceiptType.Receipt;

    public string? Cashier { get; set; }

    public string? CheckoutPoint { get; set; }

    public int? OrderNo { get; set; }

    public string? CustomerNo { get; set; }

    public IEnumerable<CreateReceiptItemDto> Items { get; set; } = null!;

    public IEnumerable<ReceiptChargeDto> Charges { get; set; } = null!;

    public IEnumerable<ReceiptDiscountDto> Discounts { get; set; } = null!;

    public PaymentDetailsDto? Payment { get; set; }

    public string? Signature { get; set; }

    public Dictionary<string, string> CustomFields { get; set; } = new Dictionary<string, string>();
}

public class CreateCustomFieldDto
{
    public string CustomFieldId { get; set; } = null!;

    public string Value { get; set; } = null!;
}

public class CreateReceiptItemDto
{
    public string? Description { get; set; } = null!;

    public string? ItemId { get; set; }

    public string? Unit { get; set; }

    public decimal? Price { get; set; }

    public double? VatRate { get; set; }

    public double Quantity { get; set; }

    public IEnumerable<ReceiptChargeDto> Charges { get; set; } = null!;

    public IEnumerable<ReceiptDiscountDto> Discounts { get; set; } = null!;

    public IDictionary<string, string> CustomFields { get; set; } = null!;
}

public class PaymentDetailsDto
{
    public decimal? Paid { get; set; }

    public decimal? Returned { get; set; }
}

public class ReceiptChargeDto
{
    public Guid Id { get; set; }

    public string Description { get; set; } = null!;

    public int? Quantity { get; set; }

    public int? Limit { get; set; }

    public decimal? Amount { get; set; }

    public double? Percent { get; set; }

    public decimal Total { get; set; }

    public Guid? ChargeId { get; set; }
}

public class ReceiptDiscountDto
{
    public Guid Id { get; set; }

    public string Description { get; set; } = null!;

    public int? Quantity { get; set; }

    public int? Limit { get; set; }

    public decimal? Amount { get; set; }

    public double? Percent { get; set; }

    public decimal Total { get; set; }

    public Guid? DiscountId { get; set; }
}

public class ReceiptDto
{
    public Guid Id { get; set; }
    public int ReceiptNo { get; set; }
    public ReceiptType Type { get; set; }
    public DateTime Date { get; set; }
    public string? Cashier { get; set; }
    public string? CheckoutPoint { get; set; }
    public int? OrderNo { get; set; }
    public string? CustomerNo { get; set; }
    public IEnumerable<ReceiptItemDto> Items { get; set; } = null!;
    public IDictionary<string, ReceiptTotalDto>? Totals { get; set; }
    public decimal? SubTotal { get; set; }
    public decimal? Vat { get; set; }
    public double? VatRate { get; set; }

    public IEnumerable<ReceiptChargeDto> Charges { get; set; } = null!;
    public decimal? Charge { get; set; }

    public IEnumerable<ReceiptDiscountDto> Discounts { get; set; } = null!;
    public decimal? Discount { get; set; }

    public decimal? Rounding { get; set; }
    public decimal Total { get; set; }

    public PaymentDetailsDto? Payment { get; set; }

    public string? Signature { get; set; }

    public IDictionary<string, object> CustomFields { get; set; } = null!;
}

public class ReceiptItemDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public string? ItemId { get; set; }
    public string? Unit { get; set; }
    public decimal Price { get; set; }
    public double VatRate { get; set; }
    public double Quantity { get; set; }

    public IEnumerable<ReceiptChargeDto> Charges { get; set; } = null!;
    public decimal? Charge { get; set; }

    public IEnumerable<ReceiptDiscountDto> Discounts { get; set; } = null!;
    public decimal? Discount { get; set; }

    public decimal Total { get; set; }

    public IDictionary<string, object> CustomFields { get; set; } = null!;
}

public class ReceiptItemProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

public class ReceiptTotalDto
{
    public ReceiptTotalDto(decimal subTotal, decimal vat, decimal total)
    {
        this.SubTotal = subTotal;
        this.Vat = vat;
        this.Total = total;
    }

    public decimal SubTotal { get; set; }
    public decimal Vat { get; set; }
    public decimal Total { get; set; }
}

public enum ReceiptType
{
    Receipt,
    Return = 1,
    Refund = 1
}