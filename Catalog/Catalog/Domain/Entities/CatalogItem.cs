using System;
using System.ComponentModel.DataAnnotations.Schema;

using Catalog.Domain.Common;

namespace Catalog.Domain.Entities
{
    public class CatalogItem : AuditableEntity, ISoftDelete
    {
        public Guid Id { get; set; }

        public string ItemId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Sku { get; set; }

        public string? Code { get; set; }

        public Unit Unit { get; set; } = null!;

        public decimal Price { get; set; }

        public bool VatIncluded { get; set; }

        [ForeignKey(nameof(VatCode))]
        public VatCode VatCodeObj { get; set; } = null!;

        public int VatCode { get; set; }

        public List<CustomField> CustomFields { get; set; } = new List<CustomField>();

        public ICollection<CatalogItemCharge> Charges { get; set; } = new List<CatalogItemCharge>();

        public int? QuantityAvailable { get; set; }

        public DateTime? Deleted { get; set; }

        public string? DeletedBy { get; set; }
    }
}