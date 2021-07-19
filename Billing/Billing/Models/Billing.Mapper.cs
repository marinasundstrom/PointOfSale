namespace Billing.Models;

using Billing.Hypermedia;

using Billing.Infrastructure.Persistence;

public static partial class Mapper
{
    public static Billing.Models.Receipts Map(IEnumerable<Billing.Domain.Entities.Receipt> receipts, string[] embed)
    {
        var r = new Billing.Models.Receipts()
        {
            Embedded = new Billing.Models.ReceiptsEmbedded
            {
                Receipts = receipts.Select(i =>
                    Billing.Models.Mapper.Map(i, embed))
            },
            Links = new Dictionary<string, Link>()
        };

        return r;
    }

    public static Billing.Models.Receipt Map(Billing.Domain.Entities.Receipt receipt, string[] embed)
    {
        var r = new Billing.Models.Receipt()
        {
            Id = receipt.Id,
            ReceiptNo = receipt.ReceiptNo,
            Type = (Contracts.ReceiptType)receipt.Type,
            Date = receipt.Date,

            SubTotal = receipt.SubTotal,

            Vat = receipt.Vat,
            VatRate = receipt.VatRate,

            Discount = receipt.Discount,
            Charge = receipt.Charge,

            Rounding = receipt.Rounding,
            Total = receipt.Total,

            Totals = receipt.Totals.Any() ? receipt.Totals.ToDictionary(x => x.VatRate.ToString(), x => new ReceiptTotal()
            {
                SubTotal = x.SubTotal,
                Vat = x.Vat,
                Total = x.Total
            }) : null,

            Payment = receipt.Payment is not null ? new Billing.Contracts.PaymentDetailsDto
            {
                Paid = receipt.Payment.Paid,
                Returned = receipt.Payment.Returned
            } : null,

            Signature = receipt.Signature,

            CustomFields = receipt.CustomFields.Any() ? receipt.CustomFields.ToDictionary(x => x.CustomFieldId, x => (object)ConvertValue(x.Value)) : null,

            Embedded = embed.Length > 0 ? new Billing.Models.ReceiptEmbedded
            {
                Items = embed.Length > 0 && embed.Any(x => x == "items") ? receipt.Items.Select(i =>
                    Billing.Models.Mapper.Map(i, embed)) : null,
                Discounts = embed.Length > 0 && embed.Any(x => x == "discounts") ? receipt.Discounts.Select(i =>
                    Billing.Models.Mapper.Map(i, Array.Empty<string>())) : null,
                Charges = embed.Length > 0 && embed.Any(x => x == "charges") ? receipt.Charges.Select(i =>
                    Billing.Models.Mapper.Map(i, Array.Empty<string>())) : null
            } : null,
            Links = new Dictionary<string, Link> {
                { "self", new Link { Href = $"/api/receipts/{receipt.ReceiptNo}" } },
            }
        };

        r.Links.Add("pdf", new Link
        {
            Href = $"/api/receipts/{receipt.ReceiptNo}/html",
            Title = "Get Receipt as HTML"
        });

        r.Links.Add("html", new Link
        {
            Href = $"/api/receipts/{receipt.ReceiptNo}/pdf",
            Title = "Get Receipt as PDF"
        });

        r.Links.Add("refund", new Link
        {
            Href = $"/api/receipts/{receipt.ReceiptNo}/refund",
            Method = "post",
            Title = "Make a full refund for this receipt"
        });

        return r;
    }

    public static Billing.Models.ReceiptItem Map(Billing.Domain.Entities.ReceiptItem i, string[] embed)
    {
        return new Billing.Models.ReceiptItem()
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

            CustomFields = i.CustomFields.Any() ? i.CustomFields.ToDictionary(x => x.CustomFieldId, x => (object)ConvertValue(x.Value)) : null,

            Embedded = embed.Length > 0 ? new Billing.Models.ReceiptItemEmbedded
            {
                Discounts = embed.Length > 0 && embed.Any(x => x == "discounts") ? i.Discounts.Select(i =>
                    Billing.Models.Mapper.Map(i, Array.Empty<string>())) : null,
                Charges = embed.Length > 0 && embed.Any(x => x == "charges") ? i.Charges.Select(i =>
                    Billing.Models.Mapper.Map(i, Array.Empty<string>())) : null,
                CustomFields = embed.Length > 0 && embed.Any(x => x == "customFields") ? i.CustomFields.Select(i =>
                    Billing.Models.Mapper.Map(i, Array.Empty<string>())) : null
            } : null,
            Links = new Dictionary<string, Link> {
                { "self", new Link { Href = $"/api/receipts/{i.Receipt!.ReceiptNo}/items/{i.Id}" } }
            }
        };
    }

    static object ConvertValue(string v)
    {
        if (bool.TryParse(v, out var boolValue))
        {
            return boolValue;
        }

        if (double.TryParse(v, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var doubleValue))
        {
            return doubleValue;
        }

        if (int.TryParse(v, out var intValue))
        {
            return intValue;
        }

        return v;
    }

    public static Billing.Models.ReceiptDiscount Map(Billing.Domain.Entities.ReceiptDiscount i, string[] embed)
    {
        var c = new Billing.Models.ReceiptDiscount()
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
            Href = i.Receipt != null
                ? $"/api/receipts/{i.Receipt!.ReceiptNo}/discounts/{i.Id}"
                : $"/api/receipts/{i.ReceiptItem!.Receipt.ReceiptNo}/items/{i.ReceiptItem.Id}/discounts/{i.Id}"
        });

        return c;
    }

    public static Billing.Models.ReceiptCharge Map(Billing.Domain.Entities.ReceiptCharge i, string[] embed)
    {
        var d = new Billing.Models.ReceiptCharge()
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
            Href = i.Receipt != null
                ? $"/api/receipts/{i.Receipt!.ReceiptNo}/charges/{i.Id}"
                : $"/api/receipts/{i.ReceiptItem!.Receipt.ReceiptNo}/items/{i.ReceiptItem.Id}/charges/{i.Id}"
        });

        return d;
    }

    public static Billing.Models.CustomField Map(Billing.Domain.Entities.CustomField i, string[] embed)
    {
        var d = new Billing.Models.CustomField()
        {
            Name = i.CustomFieldId,
            Value = i.Value
        };

        /*
        d.Links.Add("self", new Link
        {
            Href = i.Receipt != null
                ? $"/api/receipts/{i.Receipt!.ReceiptNo}/charges/{i.Id}"
                : $"/api/receipts/{i.ReceiptItem!.Receipt.ReceiptNo}/items/{i.ReceiptItem.Id}/charges/{i.Id}"
        });
        */

        return d;
    }
}