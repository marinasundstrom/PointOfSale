namespace WebApi.Receipts;

using System.Runtime.Serialization;

using Newtonsoft.Json;

using WebApi.Hal;
using WebApi.Shared;

public class Receipts : Resource<ReceiptsEmbedded>
{
    public int Count { get; set; }

    public int Total { get; set; }
}

public class ReceiptsEmbedded
{
    [JsonProperty("receipts", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<Receipt>? Receipts { get; set; }
}

public class Receipt : Resource<ReceiptEmbedded>
{
    public Guid Id { get; set; }
    public int ReceiptNo { get; set; }
    public Billing.Contracts.ReceiptType Type { get; set; }
    public DateTime Date { get; set; }

    public decimal? SubTotal { get; set; }

    [JsonProperty("vat", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Vat { get; set; }

    [JsonProperty("vatRate", NullValueHandling = NullValueHandling.Ignore)]
    public double? VatRate { get; set; }

    [JsonProperty("discount", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Discount { get; set; }

    [JsonProperty("charge", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Charge { get; set; }

    [JsonProperty("rounding", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Rounding { get; set; }
    public decimal Total { get; set; }

    [JsonProperty("totals", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, ReceiptTotal>? Totals { get; set; }

    public Billing.Contracts.PaymentDetailsDto? Payment { get; set; }

    public string? Signature { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object>? CustomFields { get; set; }
}

public class ReceiptEmbedded
{
    [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<ReceiptItem>? Items { get; set; }

    [JsonProperty("discounts", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<ReceiptDiscount>? Discounts { get; set; }

    [JsonProperty("charges", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<ReceiptCharge>? Charges { get; set; }

    [JsonProperty("customFields", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<CustomField>? CustomFields { get; set; }
}

public class ReceiptItem : Resource<ReceiptItemEmbedded>
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public string? ItemId { get; set; }
    public string? Unit { get; set; }
    public decimal Price { get; set; }
    public double VatRate { get; set; }
    public double Quantity { get; set; }

    [JsonProperty("discount", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Discount { get; set; }

    [JsonProperty("charge", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Charge { get; set; }

    public decimal Total { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object>? CustomFields { get; set; }
}

public class ReceiptItemEmbedded
{
    [JsonProperty("discounts", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<ReceiptDiscount>? Discounts { get; set; }

    [JsonProperty("charges", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<ReceiptCharge>? Charges { get; set; }

    [JsonProperty("customFields", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<CustomField>? CustomFields { get; set; }
}

public class ReceiptTotal
{
    public decimal SubTotal { get; set; }

    public decimal Vat { get; set; }

    public decimal Total { get; set; }
}

public class ReceiptDiscount : Resource
{
    public Guid Id { get; set; }

    public string Description { get; set; } = null!;

    [JsonProperty("discountId", NullValueHandling = NullValueHandling.Ignore)]
    public Guid? DiscountId { get; set; }

    [JsonProperty("amount", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Amount { get; set; }

    [JsonProperty("percent", NullValueHandling = NullValueHandling.Ignore)]
    public double? Percent { get; set; }

    [JsonProperty("quantity", NullValueHandling = NullValueHandling.Ignore)]
    public int? Quantity { get; set; }

    public decimal Total { get; set; }
}

public class ReceiptCharge : Resource
{
    public Guid Id { get; set; }

    public string Description { get; set; } = null!;

    [JsonProperty("chargeId", NullValueHandling = NullValueHandling.Ignore)]
    public Guid? ChargeId { get; set; }

    [JsonProperty("amount", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Amount { get; set; }

    [JsonProperty("percent", NullValueHandling = NullValueHandling.Ignore)]
    public double? Percent { get; set; }

    [JsonProperty("quantity", NullValueHandling = NullValueHandling.Ignore)]
    public int? Quantity { get; set; }

    public decimal Total { get; set; }
}