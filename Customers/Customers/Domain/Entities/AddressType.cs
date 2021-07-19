using System;

using Customers.Domain.Common;

namespace Customers.Domain.Entities
{
    public class AddressType
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
    }
}