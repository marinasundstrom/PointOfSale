using System;

using Customers.Domain.Common;

namespace Customers.Domain.Entities
{
    public class EmailAddressType
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
    }
}