using Billing.Domain.Entities.Common;

using Microsoft.EntityFrameworkCore;

namespace Billing.Domain.Entities;

public class CustomField : AuditableEntity
{
    public Guid Id { get; set; }

    public Receipt? Receipt { get; set; }

    public ReceiptItem? ReceiptItem { get; set; }

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