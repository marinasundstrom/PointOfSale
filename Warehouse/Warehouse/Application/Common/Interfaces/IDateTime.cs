using System;

using Warehouse.Contracts;
using Warehouse.Domain.Common;

namespace Warehouse.Application.Common.Interfaces
{
    public interface IDateTime
    {
        DateTime Now { get; }
    }
}