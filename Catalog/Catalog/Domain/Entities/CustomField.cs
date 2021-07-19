using Microsoft.EntityFrameworkCore;

using Catalog.Domain.Common;

namespace Catalog.Domain.Entities
{
    public class CustomField : AuditableEntity
    {
        public Guid Id { get; set; }

        public CatalogItem? Item { get; set; }

        public CustomFieldDefinition Definition { get; set; } = null!;

        public string CustomFieldId { get; set; } = null!;

        public string Value { get; set; } = null!;
    }

    public class CustomFieldDefinition : AuditableEntity
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public CustomFieldType Type { get; set; }
    }

    public enum CustomFieldType
    {
        String,
        Integer
    }
}