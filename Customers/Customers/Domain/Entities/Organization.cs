using System;
using System.Collections.Generic;

using Customers.Domain.Common;

namespace Customers.Domain.Entities
{
    public class Organization : AuditableEntity
    {
        public Guid Id { get; set; }

        public int CustomerNo { get; set; }

        public string Name { get; set; } = null!;

        public string OrgNo { get; set; } = null!;

        public string VatNo { get; set; } = null!;

        public DateTime? Ended { get; set; }

        public string? Notes { get; set; }

        public ICollection<Address> Addresses { get; set; } = new List<Address>()!;

        public ICollection<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>()!;

        public ICollection<EmailAddress> EmailAddresses { get; set; } = new List<EmailAddress>()!;

        public ICollection<Person> Persons { get; set; } = new List<Person>()!;

        public List<CustomField> CustomFields { get; set; } = new List<CustomField>();
    }
}