namespace WebApi.Receipts;

using Billing.Contracts;

using WebApi.Hal;
using WebApi.Hal.Forms;

public partial class Mapper
{
    private readonly UrlBuilder urlBuilder;

    public Mapper(UrlBuilder urlBuilder)
    {
        this.urlBuilder = urlBuilder;
    }

    public WebApi.Receipts.Receipts Map(IEnumerable<ReceiptDto> receipts, string[] embed)
    {
        var r = new WebApi.Receipts.Receipts()
        {
            Embedded = new WebApi.Receipts.ReceiptsEmbedded
            {
                Receipts = receipts.Select(i =>
                    Map(i, embed))
            },
            Links = new Dictionary<string, Link>()
        };

        return r;
    }

    public WebApi.Receipts.Receipt Map(ReceiptDto receipt, string[] embed)
    {
        var r = new WebApi.Receipts.Receipt()
        {
            Id = receipt.Id,
            ReceiptNo = receipt.ReceiptNo,
            Type = (Billing.Contracts.ReceiptType)receipt.Type,
            Date = receipt.Date,

            SubTotal = receipt.SubTotal,

            Vat = receipt.Vat,
            VatRate = receipt.VatRate,

            Discount = receipt.Discount,
            Charge = receipt.Charge,

            Rounding = receipt.Rounding,
            Total = receipt.Total,

            Totals = (receipt.Totals != null && receipt.Totals.Any()) ? receipt.Totals.ToDictionary(x => x.Key.ToString(), x => new ReceiptTotal()
            {
                SubTotal = x.Value.SubTotal,
                Vat = x.Value.Vat,
                Total = x.Value.Total
            }) : null,

            Payment = receipt.Payment is not null ? new Billing.Contracts.PaymentDetailsDto
            {
                Paid = receipt.Payment.Paid,
                Returned = receipt.Payment.Returned
            } : null,

            Signature = receipt.Signature,

            CustomFields = receipt.CustomFields.Any() ? receipt.CustomFields.ToDictionary(x => x.Key, x => x.Value) : null,

            Embedded = embed.Length > 0 ? new WebApi.Receipts.ReceiptEmbedded
            {
                Items = embed.Length > 0 && (embed.Contains("all") || embed.Contains("items")) ? receipt.Items.Select(i =>
                    Map(receipt.ReceiptNo, i, embed)) : null,

                Discounts = embed.Length > 0 && (embed.Contains("all") || embed.Contains("discounts")) ? receipt.Discounts.Select(i =>
                    Map(receipt.ReceiptNo, null, i, Array.Empty<string>())) : null,

                Charges = embed.Length > 0 && (embed.Contains("all") || embed.Contains("charges")) ? receipt.Charges.Select(i =>
                    Map(receipt.ReceiptNo, null, i, Array.Empty<string>())) : null,

                CustomFields = embed.Length > 0 && embed.Any(x => x == "customFields") ? receipt.CustomFields.Select(cf =>
                    Map(receipt.ReceiptNo, null, cf, Array.Empty<string>())) : null
            } : null,

            Links = new Dictionary<string, Link> {
                { "self", new Link { Href = $"{urlBuilder.GetHostUrl()}receipts/{receipt.ReceiptNo}" } },
            }
        };

        if (receipt.CustomFields.TryGetValue("orderNo", out var orderNo))
        {
            r.Links.Add("order", new Link
            {
                Href = $"{urlBuilder.GetHostUrl()}orders/{orderNo}",
                Title = "Get order"
            });
        }

        r.Links.Add("pdf", new Link
        {
            Href = $"{urlBuilder.GetHostUrl()}receipts/{receipt.ReceiptNo}/html",
            Title = "Get Receipt as HTML"
        });

        r.Links.Add("html", new Link
        {
            Href = $"{urlBuilder.GetHostUrl()}receipts/{receipt.ReceiptNo}/pdf",
            Title = "Get Receipt as PDF"
        });

        r.Templates = new Dictionary<string, Template>();

        r.Templates.Add("refund", new Template
        {
            Title = "Make a full refund for this receipt",
            Method = "POST",
            Target = $"{urlBuilder.GetHostUrl()}receipts/{receipt.ReceiptNo}/refund",
        });

        /*

        r.Templates.Add("addCustomField", new Template
        {
            Title = "Add custom field",
            Method = "POST",
            Properties = new List<Property>() {
                new Property() {
                    Name = "name",
                    Prompt = "Name",
                    Required = true
                },
                new Property() {
                    Name = "value",
                    Prompt = "Value",
                    Required = true
                },
            },
            Target = $"{urlBuilder.GetHostUrl()}receipts/{receipt.ReceiptNo}/customFields",
        });

        */

        return r;
    }

    public WebApi.Receipts.ReceiptItem Map(int receiptNo, ReceiptItemDto i, string[] embed)
    {
        var r = new WebApi.Receipts.ReceiptItem()
        {
            Id = i.Id,
            ItemId = i.ItemId,
            Description = i.Description,
            Unit = i.Unit,
            Price = i.Price,
            VatRate = i.VatRate,
            Quantity = i.Quantity,
            Charge = i.Charge,
            Discount = i.Discount,
            Total = i.Total,

            CustomFields = i.CustomFields.Any() ? i.CustomFields.ToDictionary(x => x.Key, x => x.Value) : null,

            Embedded = embed.Length > 0 ? new WebApi.Receipts.ReceiptItemEmbedded
            {
                Discounts = embed.Length > 0 && embed.Any(x => x == "discounts") ? i.Discounts.Select(i =>
                    Map(receiptNo, i.Id, i, Array.Empty<string>())) : null,

                Charges = embed.Length > 0 && embed.Any(x => x == "charges") ? i.Charges.Select(i =>
                    Map(receiptNo, i.Id, i, Array.Empty<string>())) : null,

                CustomFields = embed.Length > 0 && embed.Any(x => x == "customFields") ? i.CustomFields.Select(cf =>
                    Map(receiptNo, i.Id, cf, Array.Empty<string>())) : null
            } : null,

            Links = new Dictionary<string, Link> {
                { "self", new Link { Href = $"{urlBuilder.GetHostUrl()}receipts/{receiptNo}/items/{i.Id}" } }
            }
        };

        r.Links.Add("item", new Link { Href = $"{urlBuilder.GetHostUrl()}catalog/items/{i.ItemId}" });

        return r;
    }

    public WebApi.Receipts.ReceiptDiscount Map(int? receiptNo, Guid? receiptItemId, ReceiptDiscountDto i, string[] embed)
    {
        var c = new WebApi.Receipts.ReceiptDiscount()
        {
            Id = i.Id,
            Description = i.Description,
            DiscountId = i.DiscountId,
            Amount = i.Amount,
            Percent = i.Percent,
            Quantity = i.Quantity,
            Total = i.Total
        };

        c.Links.Add("self", new Link
        {
            Href = receiptItemId == null
                ? $"{urlBuilder.GetHostUrl()}receipts/{receiptNo}/discounts/{i.Id}"
                : $"{urlBuilder.GetHostUrl()}receipts/{receiptNo}/items/{receiptItemId}/discounts/{i.Id}"
        });

        return c;
    }

    public WebApi.Receipts.ReceiptCharge Map(int? receiptNo, Guid? receiptItemId, ReceiptChargeDto i, string[] embed)
    {
        var d = new WebApi.Receipts.ReceiptCharge()
        {
            Id = i.Id,
            Description = i.Description,
            ChargeId = i.ChargeId,
            Amount = i.Amount,
            Percent = i.Percent,
            Quantity = i.Quantity,
            Total = i.Total
        };

        d.Links.Add("self", new Link
        {
            Href = receiptItemId == null
                ? $"{urlBuilder.GetHostUrl()}receipts/{receiptNo}/charges/{i.Id}"
                : $"{urlBuilder.GetHostUrl()}receipts/{receiptNo}/items/{receiptItemId}/charges/{i.Id}"
        });

        return d;
    }

    public WebApi.Shared.CustomField Map(int? receiptNo, Guid? receiptItemId, KeyValuePair<string, object> i, string[] embed)
    {
        var d = new WebApi.Shared.CustomField()
        {
            Name = i.Key,
            Value = i.Value
        };

        d.Links.Add("self", new Link
        {
            Href = receiptItemId == null
                ? $"{urlBuilder.GetHostUrl()}receipts/{receiptNo}/customFields/{i.Key}"
                : $"{urlBuilder.GetHostUrl()}receipts/{receiptNo}/items/{receiptItemId}/customFields/{i.Key}"
        });

        if (d.Templates == null)
        {
            d.Templates = new Dictionary<string, Template>();
        }

        /*

        d.Templates.Add("delete", new Template
        {
            Title = "Delete custom field",
            Method = "DELETE",
            Target = receiptItemId == null
                ? $"{urlBuilder.GetHostUrl()}receipts/{receiptNo}/customFields/{i.Key}"
                : $"{urlBuilder.GetHostUrl()}receipts/{receiptNo}/items/{receiptItemId}/customFields/{i.Key}"
        });

        */

        return d;
    }
}