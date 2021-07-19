using System.Collections.Generic;

using Carts.Domain.Entities.Common;

using Microsoft.EntityFrameworkCore;

namespace Carts.Domain.ValueObjects
{
    [Owned]
    public class CartsDetails : ValueObject
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Ssn { get; set; }

        public string? OrganizationName { get; set; }
        public string? organizationNo { get; set; }
        public string? VatNo { get; set; }

        public Address Address { get; set; } = null!;
        //public PaymentDetails Payment { get; set; } = null!;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
            yield return Ssn!;
            yield return OrganizationName!;
            yield return organizationNo!;
            yield return VatNo!;
            yield return Address;
        }
    }
}