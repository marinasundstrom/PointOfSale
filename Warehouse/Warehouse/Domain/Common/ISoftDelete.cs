using System;

namespace Warehouse.Domain.Common
{
    public interface ISoftDelete
    {
        DateTime? Deleted { get; set; }

        string? DeletedBy { get; set; }
    }
}