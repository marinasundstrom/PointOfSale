using System;

namespace Billing.Domain.Entities.Common;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }

    string? DeletedBy { get; set; }
}