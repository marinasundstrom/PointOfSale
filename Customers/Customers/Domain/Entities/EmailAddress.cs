using System;

namespace Customers.Domain.Entities
{
    public class EmailAddress
    {
        public Guid Id { get; set; }

        public EmailAddressType Type { get; set; } = null!;

        public Person? Person { get; set; }

        public Organization? Organization { get; set; }

        public string? Description { get; set; }

        public string Address { get; set; } = null!;
    }
}