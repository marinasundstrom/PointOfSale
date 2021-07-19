using System;
using System.Collections.Generic;

namespace Customers
{
    public class PersonDto
    {
        public Guid Id { get; set; }

        public int? CustomerNo { get; set; }

        public string FirstName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = null!;

        public string? Title { get; set; }

        public string? Organization { get; set; }

        public string? Gender { get; set; }

        public string Ssn { get; set; } = null!;

        public DateTime? Deceased { get; set; }

        public string? Notes { get; set; }

        public IEnumerable<AddressDto> Addresses { get; set; } = null!;

        public IEnumerable<EmailAddressDto> EmailAddresses { get; set; } = null!;

        public IEnumerable<PhoneNumberDto> PhoneNumbers { get; set; } = null!;
    }
}