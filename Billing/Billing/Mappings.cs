using Billing.Contracts;
using Billing.Domain.Entities;

using OrderPriceCalculator;

namespace Billing;

public static class Mappings
{
    public static ReceiptDto CreateReceiptDto(Receipt receipt)
    {
        var dto = new ReceiptDto()
        {
            Id = receipt.Id,
            ReceiptNo = receipt.ReceiptNo,
            Date = receipt.Date,
            Type = (Contracts.ReceiptType)receipt.Type,
            //CustomerNo = receipt.CustomerNo,
            //OrderNo = receipt.OrderNo,
            Items = receipt.Items.OrderBy(i => i.Created).Select(Mappings.CreateReceiptItemDto),
            Totals = receipt.Vat(),
            SubTotal = receipt.SubTotal,
            Vat = receipt.Vat,
            VatRate = receipt.VatRate,
            Charges = receipt.Charges.Any() ? receipt.Charges.Select(CreateReceiptChargesDto) : Array.Empty<ReceiptChargeDto>(),
            Charge = receipt.Charge,
            Discounts = receipt.Discounts.Any() ? receipt.Discounts.Select(CreateReceiptDiscountsDto) : Array.Empty<ReceiptDiscountDto>(),
            Discount = receipt.Discount,
            Rounding = receipt.Rounding,
            Total = receipt.Total,
            Payment = receipt.Payment is not null ? new PaymentDetailsDto
            {
                Paid = receipt.Payment.Paid,
                Returned = receipt.Payment.Returned
            } : null,
            Signature = receipt.Signature,
            CustomFields = receipt.CustomFields.ToDictionary(f => f.CustomFieldId, f => (object)ConvertValue(f.Value))
        };

        return dto;
    }

    public static object ConvertValue(string v)
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

    public static ReceiptChargeDto CreateReceiptChargesDto(ReceiptCharge arg)
    {
        return new ReceiptChargeDto()
        {
            Id = arg.Id,
            Amount = arg.Amount,
            Quantity = arg.Quantity,
            Limit = arg.Limit,
            Percent = (double?)arg.Percent,
            Total = arg.Total,
            Description = arg.Description ?? string.Empty,
            //ChargeId = arg.ChargeId
        };
    }

    public static ReceiptDiscountDto CreateReceiptDiscountsDto(ReceiptDiscount arg)
    {
        return new ReceiptDiscountDto()
        {
            Id = arg.Id,
            Amount = arg.Amount,
            Quantity = arg.Quantity,
            Limit = arg.Limit,
            Percent = (double?)arg.Percent,
            Total = arg.Total,
            Description = arg.Description,
            DiscountId = arg.DiscountId
        };
    }

    /*

    public static Dictionary<string, ReceiptTotalDto> CreateReceiptVatDtos(Receipt receipt)
    {
        var arg = receipt.Totals.OrderBy(i => i.VatRate);

        var result = arg.ToDictionary(
            x => x.VatRate.ToString(), x => new ReceiptTotalDto(x.SubTotal, x.Vat, x.Total));

        if (receipt.VatRate is not null)
        {
            var label = (receipt.VatRate.GetValueOrDefault() * 100).ToString();

            return new(new KeyValuePair<string, ReceiptTotalDto>[] { new (
                label,
                new ReceiptTotalDto(receipt.SubTotal, receipt.Vat, receipt.Total - receipt.Rounding - receipt.Discounts.Sum(x => x.Total).GetValueOrDefault()))
            });
        }
        else
        {
            KeyValuePair<string, ReceiptTotalDto> total = new(
                "Total",
                new ReceiptTotalDto(result.Sum(x => x.Value.SubTotal), result.Sum(x => x.Value.Vat), result.Sum(x => x.Value.Total))
            );

            return new(result.ToList().Concat(new[] { total }));
        }
    }

    */

    public static ReceiptItemDto CreateReceiptItemDto(ReceiptItem i)
    {
        return new ReceiptItemDto
        {
            Id = i.Id,
            ItemId = i.ItemId,
            Description = i.Description,
            Unit = i.Unit,
            Price = i.Price,
            VatRate = i.VatRate,
            Quantity = i.Quantity,
            Charges = i.Charges.Any() ? i.Charges.Select(CreateReceiptChargesDto) : Array.Empty<ReceiptChargeDto>(),
            Charge = i.Charge,
            Discounts = i.Discounts.Any() ? i.Discounts.Select(Mappings.CreateReceiptDiscountsDto) : Array.Empty<ReceiptDiscountDto>(),
            Discount = i.Discount,
            Total = i.Total,
            CustomFields = i.CustomFields.ToDictionary(f => f.CustomFieldId, f => (object)ConvertValue(f.Value))
        };
    }
}