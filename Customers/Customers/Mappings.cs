using System.Linq;

using Customers.Domain.Entities;

namespace Customers
{
    public static class Mappings
    {
        public static AddressDto ToDto(this Address address)
        {
            return new AddressDto
            {
                Id = address.Id,

                Type = new AddressTypeDto
                {
                    Id = address.Type.Id,
                    Name = address.Type.Name
                },

                Description = address.Description,

                // Street
                Thoroughfare = address.Thoroughfare,

                // Street number
                Premises = address.Premises,

                // Suite
                SubPremises = address.SubPremises,

                PostalCode = address.PostalCode,

                // Town or City
                Locality = address.Locality,

                // County
                SubAdministrativeArea = address.SubAdministrativeArea,

                // State
                AdministrativeArea = address.AdministrativeArea,

                Country = address.Country
            };
        }

        public static EmailAddressDto ToDto(this EmailAddress emailAddress)
        {
            return new EmailAddressDto
            {
                Id = emailAddress.Id,

                Description = emailAddress.Description,

                Address = emailAddress.Address,
            };
        }

        public static PhoneNumberDto ToDto(this PhoneNumber phoneNumber)
        {
            return new PhoneNumberDto
            {
                Id = phoneNumber.Id,

                Description = phoneNumber.Description,

                Number = phoneNumber.Number,
            };
        }

        public static PersonDto ToDto(this Person person)
        {
            return new PersonDto
            {
                Id = person.Id,
                CustomerNo = person.CustomerNo,
                FirstName = person.FirstName,
                MiddleName = person.MiddleName,
                LastName = person.LastName,
                Title = person.Title,
                Organization = person.Organization?.Name,
                Gender = person.Gender.ToString(),
                Ssn = person.Ssn,
                Deceased = person.Deceased,
                Notes = person.Notes,
                Addresses = person.Addresses.Select(Mappings.ToDto),
                EmailAddresses = person.EmailAddresses.Select(Mappings.ToDto),
                PhoneNumbers = person.PhoneNumbers.Select(Mappings.ToDto)
            };
        }

        public static OrganizationDto ToDto(this Organization organization)
        {
            return new OrganizationDto
            {
                Id = organization.Id,
                CustomerNo = organization.CustomerNo,
                Name = organization.Name,
                OrgNo = organization.OrgNo,
                VatNo = organization.VatNo,
                Ended = organization.Ended,
                Notes = organization.Notes,
                Addresses = organization.Addresses.Select(Mappings.ToDto),
                EmailAddresses = organization.EmailAddresses.Select(Mappings.ToDto),
                PhoneNumbers = organization.PhoneNumbers.Select(Mappings.ToDto),
                Persons = organization.Persons.Select(Mappings.ToDto)
            };
        }
    }
}