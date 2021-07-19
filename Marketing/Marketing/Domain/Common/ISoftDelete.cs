using System;

namespace Marketing.Domain.Common;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }

    string? DeletedBy { get; set; }
}