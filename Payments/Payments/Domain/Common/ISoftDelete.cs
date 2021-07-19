using System;

namespace Payments.Domain.Common
{
    public interface ISoftDelete
    {
        DateTime? Deleted { get; set; }

        string? DeletedBy { get; set; }
    }
}