namespace WebApi.Carts;

using System.Runtime.Serialization;

using Newtonsoft.Json;

using WebApi.Hal;
using WebApi.Shared;

public class Carts : Resource<CartsEmbedded>
{
    public int Count { get; set; }

    public int Total { get; set; }
}

public class CartsEmbedded
{
    [JsonProperty("carts", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<Cart>? Carts { get; set; }
}

public class Cart : Resource<CartEmbedded>
{
    public Guid Id { get; set; }

    public string? Tag { get; set; }

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
    public Dictionary<string, CartTotal>? Totals { get; set; }

    //public Sales.Contracts.PaymentDetailsDto? Payment { get; set; }

    //public string? Signature { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object>? CustomFields { get; set; }
}

public class CartEmbedded
{
    [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<CartItem>? Items { get; set; }

    [JsonProperty("discounts", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<CartDiscount>? Discounts { get; set; }

    [JsonProperty("charges", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<CartCharge>? Charges { get; set; }

    [JsonProperty("customFields", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<CustomField>? CustomFields { get; set; }
}

public class CartItem : Resource<CartItemEmbedded>
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

public class CartItemEmbedded
{
    [JsonProperty("discounts", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<CartDiscount>? Discounts { get; set; }

    [JsonProperty("charges", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<CartCharge>? Charges { get; set; }

    [JsonProperty("customFields", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<CustomField>? CustomFields { get; set; }
}

public class CartTotal
{
    public decimal SubTotal { get; set; }

    public decimal Vat { get; set; }

    public decimal Total { get; set; }
}

public class CartDiscount : Resource
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

public class CartCharge : Resource
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