using System;

namespace Customers
{
    public class PhoneNumberDto
    {
        public Guid Id { get; set; }

        public string? Description { get; set; }

        public string Number { get; set; } = null!;
    }
}