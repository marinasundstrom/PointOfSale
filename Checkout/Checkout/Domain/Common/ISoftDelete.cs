using System;

namespace Checkout.Domain.Entities.Common
{
    public interface ISoftDelete
    {
        DateTime? Deleted { get; set; }

        string? DeletedBy { get; set; }
    }
}