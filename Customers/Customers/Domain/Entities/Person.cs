﻿using System;
using System.Collections.Generic;

using Customers.Domain.Common;
using Customers.Domain.Enums;

namespace Customers.Domain.Entities
{
    public class Person : AuditableEntity
    {
        public Guid Id { get; set; }

        public Organization? Organization { get; set; }

        public int? CustomerNo { get; set; }

        public string FirstName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = null!;

        public string? Title { get; set; }

        public Gender Gender { get; set; }

        public string Ssn { get; set; } = null!;

        public DateTime? Deceased { get; set; }

        public string? Notes { get; set; }

        public ICollection<Address> Addresses { get; set; } = new List<Address>()!;

        public ICollection<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>()!;

        public ICollection<EmailAddress> EmailAddresses { get; set; } = new List<EmailAddress>()!;

        public List<CustomField> CustomFields { get; set; } = new List<CustomField>();
    }
}