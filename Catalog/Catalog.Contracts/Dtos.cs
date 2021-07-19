using System;

namespace Catalog.Contracts
{
    public class CatalogItemDto
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Sku { get; set; }

        public string? Code { get; set; }

        public UnitDto Unit { get; set; } = null!;

        public decimal Price { get; set; }

        public double VatRate { get; set; }

        public bool VatIncluded { get; set; }

        public decimal Vat { get; set; }

        public IEnumerable<CatalogItemChargeDto> Charges { get; set; } = null!;

        public Dictionary<string, object> CustomFields { get; set; } = null!;

        public int? QuantityAvailable { get; set; }
    }

    public class UnitDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Code { get; set; }
    }

    public class CatalogItemChargeDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        // public Guid? ChargeId { get; set; }

        // Quantity
        // Limit

        public decimal? Amount { get; set; }

        public double? Percent { get; set; }
    }
}