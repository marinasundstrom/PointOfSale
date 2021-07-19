using System;

using Catalog.Domain.Common;

namespace Catalog.Domain.Entities
{
    public class VatCode : AuditableEntity, ISoftDelete
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public double VatRate { get; set; }

        public DateTime? Deleted { get; set; }

        public string? DeletedBy { get; set; }
    }
}