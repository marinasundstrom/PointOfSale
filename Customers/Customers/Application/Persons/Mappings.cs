using System.Linq;

using Customers.Domain.Entities;

using Customers.Contracts.Persons;

namespace Customers.Application.Persons
{
    public static class Mappings
    {
        /*
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

        */

        public static Customers.Contracts.Persons.PersonDto ToDto(this Person person)
        {
            return new Customers.Contracts.Persons.PersonDto
            {
                Id = person.Id,
                CustomerNo = person.CustomerNo,
                FirstName = person.FirstName,
                MiddleName = person.MiddleName,
                LastName = person.LastName,
                Title = person.Title,
                //Organization = person.Organization?.Name,
                //Gender = person.Gender.ToString(),
                Ssn = person.Ssn,
                Deceased = person.Deceased,
                Notes = person.Notes,
                /*
                Addresses = person.Addresses.Select(Mappings.ToDto),
                EmailAddresses = person.EmailAddresses.Select(Mappings.ToDto),
                PhoneNumbers = person.PhoneNumbers.Select(Mappings.ToDto)
                */

                CustomFields = person.CustomFields.ToDictionary(f => f.CustomFieldId, f => (object)ConvertValue(f.Value))
            };
        }

        public static object ConvertValue(string v)
        {
            if (bool.TryParse(v, out var boolValue))
            {
                return boolValue;
            }

            if (double.TryParse(v, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var doubleValue))
            {
                return doubleValue;
            }

            if (int.TryParse(v, out var intValue))
            {
                return intValue;
            }

            return v;
        }
    }
}