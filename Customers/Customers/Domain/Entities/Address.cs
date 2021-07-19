using System;

using Customers.Domain.Common;

namespace Customers.Domain.Entities
{
    public class Address : IAddress
    {
        public Guid Id { get; set; }

        public AddressType Type { get; set; } = null!;

        public string? Description { get; set; }

        public Person? Person { get; set; }

        public Organization? Organization { get; set; }

        // Street
        public string Thoroughfare { get; set; } = null!;

        // Street number
        public string? Premises { get; set; }

        // Suite
        public string? SubPremises { get; set; }

        public string PostalCode { get; set; } = null!;

        // Town or City
        public string Locality { get; set; } = null!;

        // County
        public string SubAdministrativeArea { get; set; } = null!;

        // State
        public string AdministrativeArea { get; set; } = null!;

        public string Country { get; set; } = null!;
    }
}