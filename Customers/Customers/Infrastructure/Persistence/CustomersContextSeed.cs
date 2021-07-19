using System;
using System.Linq;
using System.Threading.Tasks;

using Customers.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

namespace Customers.Infrastructure.Persistence
{
    public static class CustomersContextSeed
    {
        public static async Task SeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<CustomersContext>();

            //await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var billingAddressType = new AddressType()
            {
                Id = Guid.NewGuid(),
                Name = "Billing address",
            };

            var deliveryAddressType = new AddressType()
            {
                Id = Guid.NewGuid(),
                Name = "Delivery address",
            };

            var addressTypes = new[]
                {
                    billingAddressType,
                    deliveryAddressType,
                }.ToList();

            context.AddRange(addressTypes);

            await context.SaveChangesAsync();

            if (!context.CustomFieldDefinitions.Any())
            {
                context.CustomFieldDefinitions.Add(new CustomFieldDefinition
                {
                    Id = "favoritePet",
                    Name = "Favorite Pet",
                    Type = CustomFieldType.String
                });

                await context.SaveChangesAsync();
            }

            var phoneNumberType = new PhoneNumberType()
            {
                Id = Guid.NewGuid(),
                Name = "Contact",
            };

            var phoneNumberTypes = new[]
                {
                    phoneNumberType,
                }.ToList();

            context.AddRange(phoneNumberTypes);

            var emailAddressType = new EmailAddressType()
            {
                Id = Guid.NewGuid(),
                Name = "Contact",
            };

            var emailAddressTypes = new[]
                {
                    emailAddressType,
                }.ToList();

            context.AddRange(emailAddressTypes);

            await context.SaveChangesAsync();

            var addresses = new[]
                {
                    new Address()
                    {
                        Id = Guid.NewGuid(),
                        Type = billingAddressType,
                        Thoroughfare = "Stenbäcken",
                        Premises = "3",
                        PostalCode = "123",
                        Locality = "Sandskogen",
                        SubAdministrativeArea = "Land kommun",
                        AdministrativeArea = "Skåne län",
                        Country = "Sweden"
                    }
                }.ToList();

            context.AddRange(addresses);

            await context.SaveChangesAsync();

            var person = new Domain.Entities.Person()
            {
                CustomerNo = 1,
                FirstName = "John",
                LastName = "Doe",
                Gender = Domain.Enums.Gender.Male,
                Ssn = "234433",
                Addresses = addresses
            };

            person.CustomFields.Add(new CustomField() {
                CustomFieldId = "favoritePet",
                Value = "Buster"
            });

            context.Persons.Add(person);

            addresses = new[]
                {
                    new Address()
                    {
                        Id = Guid.NewGuid(),
                        Type = billingAddressType,
                        Thoroughfare = "Haga",
                        Premises = "2",
                        PostalCode = "123",
                        Locality = "Barkaby",
                        SubAdministrativeArea = "Barkaby kommun",
                        AdministrativeArea = "Stockholms län",
                        Country = "Sweden"
                    },
                    new Address()
                    {
                        Id = Guid.NewGuid(),
                        Type = deliveryAddressType,
                        Thoroughfare = "Haga",
                        Premises = "2",
                        PostalCode = "123",
                        Locality = "Barkaby",
                        SubAdministrativeArea = "Barkaby kommun",
                        AdministrativeArea = "Stockholms län",
                        Country = "Sweden"
                    }
                }.ToList();

            context.AddRange(addresses);

            await context.SaveChangesAsync();

            context.Persons.Add(new Domain.Entities.Person()
            {
                CustomerNo = 2,
                FirstName = "Jane",
                LastName = "Doe",
                Gender = Domain.Enums.Gender.Female,
                Ssn = "2343434",
                Addresses = addresses
            });

            addresses = new[]
                {
                    new Address()
                    {
                        Id = Guid.NewGuid(),
                        Type = billingAddressType,
                        Description = "En vanlig address",
                        Thoroughfare = "Lindvägen 2",
                        PostalCode = "653 34",
                        Locality = "Berg",
                        SubAdministrativeArea = "Lisa kommun",
                        AdministrativeArea = "Boda län",
                        Country = "Sweden"
                    }
                }.ToList();

            context.AddRange(addresses);

            await context.SaveChangesAsync();

            var organization = new Domain.Entities.Organization()
            {
                CustomerNo = 3,
                Name = "ACME Inc",
                OrgNo = "343234234",
                VatNo = "343234234-S",
                Addresses = addresses
            };

            addresses = new[]
    {
                    new Address()
                    {
                        Id = Guid.NewGuid(),
                        Type = billingAddressType,
                        Thoroughfare = "Diamond Alley",
                        Premises = "2",
                        PostalCode = "342",
                        Locality = "Riverside",
                        SubAdministrativeArea = "Bath",
                        AdministrativeArea = "Doughla",
                        Country = "Neverland"
                    }
                }.ToList();

            context.AddRange(addresses);

            await context.SaveChangesAsync();

            context.Persons.Add(new Domain.Entities.Person()
            {
                FirstName = "Moxxy",
                LastName = "Starlord",
                Gender = Domain.Enums.Gender.Other,
                Title = "Sales Representative",
                Organization = organization,
                Ssn = "2343434",
                Addresses = addresses
            });

            context.Organizations.Add(organization);

            await context.SaveChangesAsync();
        }
    }
}