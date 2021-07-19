namespace WebApi.Customers;

using System.Runtime.Serialization;

using Newtonsoft.Json;

using WebApi.Hal;
using WebApi.Shared;

public class Persons : Resource<PersonsEmbedded>
{
    public int Count { get; set; }

    public int Total { get; set; }
}

public class PersonsEmbedded
{
    [JsonProperty("persons", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<Person>? Persons { get; set; }
}

public class Person : Resource<PersonEmbedded>
{
    public Guid Id { get; set; }

    //public Organization? Organization { get; set; }

    public int? CustomerNo { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string? Title { get; set; }

    //public Gender Gender { get; set; }

    public string Ssn { get; set; } = null!;

    public DateTime? Deceased { get; set; }

    public string? Notes { get; set; }

    /*

    public ICollection<Address> Addresses { get; set; } = new List<Address>()!;

    public ICollection<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>()!;

    public ICollection<EmailAddress> EmailAddresses { get; set; } = new List<EmailAddress>()!;

    */

    [JsonExtensionData]
    public Dictionary<string, object>? CustomFields { get; set; }
}

public class PersonEmbedded
{
    [JsonProperty("charges", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<PersonCharge>? Charges { get; set; }

    [JsonProperty("customFields", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<CustomField>? CustomFields { get; set; }
}

public class PersonCharge : Resource
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    [JsonProperty("amount", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Amount { get; set; }

    [JsonProperty("percent", NullValueHandling = NullValueHandling.Ignore)]
    public double? Percent { get; set; }
}