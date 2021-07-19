using System;

using Catalog.Contracts;
using Catalog.Domain.Entities;

using OrderPriceCalculator;

namespace Catalog
{
    public static class Mappings
    {
        public static CatalogItemDto ToItemDto(this CatalogItem catalogItem)
        {
            var vatRate = catalogItem.VatCodeObj.VatRate;

            return new CatalogItemDto()
            {
                Id = catalogItem.ItemId,
                Name = catalogItem.Name,
                Description = catalogItem.Description!,
                Sku = catalogItem.Sku,
                Code = catalogItem.Code,
                Unit = new UnitDto()
                {
                    Id = catalogItem.Unit.Id,
                    Name = catalogItem.Unit.Name,
                    Code = catalogItem.Unit.Code,
                },
                Price = catalogItem.Price, /* Math.Round(catalogItem.VatIncluded
                    ? catalogItem.Price
                    : catalogItem.Price.AddVat(vatRate), 2), */
                VatRate = vatRate,
                VatIncluded = catalogItem.VatIncluded,
                Vat = Math.Round(catalogItem.VatIncluded
                    ? catalogItem.Price.GetVatIncl(vatRate)
                    : catalogItem.Price.Vat(vatRate), 2),
                Charges = catalogItem.Charges.Select(ToChargesDto),
                QuantityAvailable = catalogItem.QuantityAvailable,
                CustomFields = catalogItem.CustomFields.ToDictionary(f => f.CustomFieldId, f => (object)ConvertValue(f.Value))
            };
        }

        public static CatalogItemChargeDto ToChargesDto(this CatalogItemCharge charge)
        {
            return new CatalogItemChargeDto()
            {
                Id = charge.Id,
                Name = charge.Name,
                Description = charge.Description,

                // public Guid? ChargeId { get; set; }

                // Quantity
                // Limit

                Amount = charge.Amount,
                Percent = charge.Percent,
            };
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
    }
}