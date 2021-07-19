namespace WebApi.Catalog;

using System.Runtime.Serialization;

using Newtonsoft.Json;

using WebApi.Hal;
using WebApi.Shared;

public class Items : Resource<ItemsEmbedded>
{
    public int Count { get; set; }

    public int Total { get; set; }
}

public class ItemsEmbedded
{
    [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<Item>? Items { get; set; }
}

public class Item : Resource<ItemEmbedded>
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? Unit { get; set; }

    public decimal Price { get; set; }

    public double VatRate { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object>? CustomFields { get; set; }
}

public class ItemEmbedded
{
    [JsonProperty("charges", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<ItemCharge>? Charges { get; set; }

    [JsonProperty("customFields", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<CustomField>? CustomFields { get; set; }
}

public class ItemCharge : Resource
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    [JsonProperty("amount", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Amount { get; set; }

    [JsonProperty("percent", NullValueHandling = NullValueHandling.Ignore)]
    public double? Percent { get; set; }
}