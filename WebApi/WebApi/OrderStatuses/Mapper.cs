namespace WebApi.OrderStatuses;

using Sales.Contracts;

using WebApi.Hal;
using WebApi.Hal.Forms;

public partial class Mapper
{
    private readonly UrlBuilder urlBuilder;

    public Mapper(UrlBuilder urlBuilder)
    {
        this.urlBuilder = urlBuilder;
    }

    public WebApi.OrderStatuses.OrderStatuses Map(IEnumerable<OrderStatusDto> orderStatuses, string[] embed)
    {
        var r = new WebApi.OrderStatuses.OrderStatuses()
        {
            Embedded = new WebApi.OrderStatuses.OrderStatusesEmbedded
            {
                OrderStatuses = orderStatuses.Select(i =>
                    Map(i, embed))
            },
            Links = new Dictionary<string, Link>()
        };

        return r;
    }

    public WebApi.OrderStatuses.OrderStatus Map(OrderStatusDto orderStatus, string[] embed)
    {
        var r = new WebApi.OrderStatuses.OrderStatus()
        {
            Id = orderStatus.Id,
            Name = orderStatus.Name,

            Links = new Dictionary<string, Link> {
                { "self", new Link { Href = $"{urlBuilder.GetHostUrl()}ordersStatuses/{orderStatus.Id}" } },
            }
        };

        return r;
    }
}