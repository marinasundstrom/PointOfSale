namespace WebApi.OrderStatuses;

using System.Runtime.Serialization;

using Newtonsoft.Json;

using WebApi.Hal;
using WebApi.Shared;

public class OrderStatuses : Resource<OrderStatusesEmbedded>
{
    public int Count { get; set; }

    public int Total { get; set; }
}

public class OrderStatusesEmbedded
{
    [JsonProperty("orderStatuses", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<OrderStatus>? OrderStatuses { get; set; }
}

public class OrderStatus : Resource<OrderStatusEmbedded>
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;
}

public class OrderStatusEmbedded
{
    
}