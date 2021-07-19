using Microsoft.EntityFrameworkCore;

using Customers.Domain.Common;

namespace Customers.Domain.Entities
{
    public class CustomField : AuditableEntity
    {
        public Guid Id { get; set; }

        public Person? Person { get; set; }

        public Organization? Organization { get; set; }

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