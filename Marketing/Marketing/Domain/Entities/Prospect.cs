using System;

using Marketing.Domain.Common;

namespace Marketing.Domain.Entities;

public class Prospect : AuditableEntity, ISoftDelete
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Organization { get; set; }

    //public Address Address { get; set; }

    public DateTime? Deleted { get; set; }

    public string? DeletedBy { get; set; }
}