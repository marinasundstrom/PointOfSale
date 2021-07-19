namespace WebApi.Catalog;

using global::Catalog.Contracts;

using WebApi.Hal;
using WebApi.Hal.Forms;

public partial class Mapper
{
    private readonly UrlBuilder urlBuilder;

    public Mapper(UrlBuilder urlBuilder)
    {
        this.urlBuilder = urlBuilder;
    }

    public WebApi.Catalog.Items Map(IEnumerable<CatalogItemDto> items, string[] embed)
    {
        var r = new WebApi.Catalog.Items()
        {
            Embedded = new WebApi.Catalog.ItemsEmbedded
            {
                Items = items.Select(i =>
                    Map(i, embed))
            },
            Links = new Dictionary<string, Link>()
        };

        return r;
    }

    public WebApi.Catalog.Item Map(CatalogItemDto item, string[] embed)
    {
        var r = new WebApi.Catalog.Item()
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description ?? string.Empty,
            Unit = item.Unit?.Name,
            Price = item.Price,
            VatRate = item.VatRate,

            CustomFields = item.CustomFields.Any() ? item.CustomFields.ToDictionary(x => x.Key, x => x.Value) : null,

            Embedded = embed.Length > 0 ? new WebApi.Catalog.ItemEmbedded
            {
                Charges = embed.Length > 0 && (embed.Contains("all") || embed.Contains("charges")) ? item.Charges.Select(i =>
                    Map(item.Id, i, Array.Empty<string>())) : null,

                CustomFields = embed.Length > 0 && embed.Any(x => x == "customFields") ? item.CustomFields.Select(cf =>
                    Map(item.Id, cf, Array.Empty<string>())) : null
            } : null,

            Links = new Dictionary<string, Link> {
                { "self", new Link { Href = $"{urlBuilder.GetHostUrl()}catalog/items/{item.Id}" } },
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
            Target = $"{urlBuilder.GetHostUrl()}catalog/items/{item.Id}/customFields",
        });

        return r;
    }

    public WebApi.Catalog.ItemCharge Map(string itemId, CatalogItemChargeDto i, string[] embed)
    {
        var d = new WebApi.Catalog.ItemCharge()
        {
            Id = i.Id,
            Name = i.Name,
            Amount = i.Amount,
            Percent = i.Percent,
        };

        d.Links.Add("self", new Link
        {
            Href = $"{urlBuilder.GetHostUrl()}catalog/items/{itemId}/charges/{i.Id}"
        });

        return d;
    }

    public WebApi.Shared.CustomField Map(string itemId, KeyValuePair<string, object> i, string[] embed)
    {
        var d = new WebApi.Shared.CustomField()
        {
            Name = i.Key,
            Value = i.Value
        };

        d.Links.Add("self", new Link
        {
            Href = $"{urlBuilder.GetHostUrl()}catalog/items/{itemId}/customFields/{i.Key}"
        });

        if (d.Templates == null)
        {
            d.Templates = new Dictionary<string, Template>();
        }

        d.Templates.Add("delete", new Template
        {
            Title = "Delete custom field",
            Method = "DELETE",
            Target = $"{urlBuilder.GetHostUrl()}catalog/items/{itemId}/customFields/{i.Key}"
        });

        return d;
    }
}