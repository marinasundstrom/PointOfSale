namespace WebApi.Customers;

using global::Customers.Contracts.Persons;

using WebApi.Hal;
using WebApi.Hal.Forms;

public partial class Mapper
{
    private readonly UrlBuilder urlBuilder;

    public Mapper(UrlBuilder urlBuilder)
    {
        this.urlBuilder = urlBuilder;
    }

    public WebApi.Customers.Persons Map(IEnumerable<PersonDto> items, string[] embed)
    {
        var r = new WebApi.Customers.Persons()
        {
            Embedded = new WebApi.Customers.PersonsEmbedded
            {
                Persons = items.Select(i =>
                    Map(i, embed))
            },
            Links = new Dictionary<string, Link>()
        };

        return r;
    }

    public WebApi.Customers.Person Map(PersonDto item, string[] embed)
    {
        var r = new WebApi.Customers.Person()
        {
            Id = item.Id,
            CustomerNo = item.CustomerNo,
            //Description = item.Description!,
            FirstName = item.FirstName,
            MiddleName = item.MiddleName,
            LastName = item.LastName,
            Title = item.Title,
            Ssn = item.Ssn,
            Deceased = item.Deceased,
            Notes = item.Notes,

            CustomFields = item.CustomFields.Any() ? item.CustomFields.ToDictionary(x => x.Key, x => x.Value) : null,

            Embedded = embed.Length > 0 ? new WebApi.Customers.PersonEmbedded
            {
                CustomFields = embed.Length > 0 && embed.Any(x => x == "customFields") ? item.CustomFields.Select(cf =>
                    Map(item.Id, cf, Array.Empty<string>())) : null
            } : null,

            Links = new Dictionary<string, Link> {
                { "self", new Link { Href = $"{urlBuilder.GetHostUrl()}customers/persons/{item.Id}" } },
            }
        };

        r.Templates = new Dictionary<string, Template>();

        r.Templates.Add("addCustomField", new Template
        {
            Title = "Add custom field",
            Method = "POST",
            Properties = new List<Property>() {
                new Property() {
                    Name = "name",
                    Prompt = "Name",
                    Required = true,
                    Type = PropertyType.Text
                },
                new Property() {
                    Name = "value",
                    Prompt = "Value",
                    Required = true,
                    Type = PropertyType.Text
                },
            },
            Target = $"{urlBuilder.GetHostUrl()}customers/persons/{item.Id}/customFields",
        });


        return r;
    }

    public WebApi.Shared.CustomField Map(Guid personId, KeyValuePair<string, object> i, string[] embed)
    {
        var d = new WebApi.Shared.CustomField()
        {
            Name = i.Key,
            Value = i.Value
        };

        d.Links.Add("self", new Link
        {
            Href = $"{urlBuilder.GetHostUrl()}customers/persons/{personId}/customFields/{i.Key}"
        });

        if (d.Templates == null)
        {
            d.Templates = new Dictionary<string, Template>();
        }

        d.Templates.Add("delete", new Template
        {
            Title = "Delete custom field",
            Method = "DELETE",
            Target = $"{urlBuilder.GetHostUrl()}customers/persons/{personId}/customFields/{i.Key}"
        });

        return d;
    }
}