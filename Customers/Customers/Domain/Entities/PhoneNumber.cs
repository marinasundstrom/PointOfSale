using System;

namespace Customers.Domain.Entities
{
    public class PhoneNumber
    {
        public Guid Id { get; set; }

        public PhoneNumberType Type { get; set; } = null!;

        public Person? Person { get; set; }

        public Organization? Organization { get; set; }

        public string? Description { get; set; }

        public string Number { get; set; } = null!;
    }
}