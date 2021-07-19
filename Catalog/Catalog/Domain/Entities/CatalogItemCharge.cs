using System;
using System.ComponentModel.DataAnnotations.Schema;

using Catalog.Domain.Common;

namespace Catalog.Domain.Entities
{
    public class CatalogItemCharge : AuditableEntity, ISoftDelete
    {
        public Guid Id { get; set; }

        public CatalogItem CatalogItem { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        // public Guid? ChargeId { get; set; }

        // Quantity
        // Limit

        public decimal? Amount { get; set; }

        public double? Percent { get; set; }

        public DateTime? Deleted { get; set; }

        public string? DeletedBy { get; set; }
    }
}