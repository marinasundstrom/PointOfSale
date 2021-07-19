using System;

namespace Customers
{
    public class EmailAddressDto
    {
        public Guid Id { get; set; }

        public string? Description { get; set; }

        public string Address { get; set; } = null!;
    }
}