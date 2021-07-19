using System;
using System.Collections.Generic;

namespace Customers
{
    public class OrganizationDto
    {
        public Guid Id { get; set; }

        public int CustomerNo { get; set; }

        public string Name { get; set; } = null!;

        public string OrgNo { get; set; } = null!;

        public string VatNo { get; set; } = null!;

        public DateTime? Ended { get; set; }

        public string? Notes { get; set; }

        public IEnumerable<AddressDto> Addresses { get; set; } = null!;

        public IEnumerable<EmailAddressDto> EmailAddresses { get; set; } = null!;

        public IEnumerable<PhoneNumberDto> PhoneNumbers { get; set; } = null!;

        public IEnumerable<PersonDto> Persons { get; set; } = null!;
    }
}