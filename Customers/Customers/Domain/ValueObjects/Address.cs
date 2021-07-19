using System.Collections.Generic;

using Customers.Domain.Common;

using Microsoft.EntityFrameworkCore;

namespace Customers.Domain.ValueObjects
{
    [Owned]
    public class Address : ValueObject, IAddress
    {
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

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Thoroughfare;
            yield return Premises!;
            yield return SubPremises!;
            yield return PostalCode;
            yield return Locality;
            yield return SubAdministrativeArea;
            yield return AdministrativeArea;
            yield return Country;
        }
    }
}