namespace WebApi.OrderStatuses;

using Billing.Contracts;

using MassTransit;

using Microsoft.AspNetCore.Mvc;

using Sales.Contracts;

[ApiController]
[Route("[controller]")]
[Produces("application/prs.hal-forms+json")]
public class OrderStatusesController : ControllerBase
{
    private readonly ILogger<OrderStatusesController> _logger;
    private readonly WebApi.OrderStatuses.Mapper mapper;

    public OrderStatusesController(
        ILogger<OrderStatusesController> logger, 
        WebApi.OrderStatuses.Mapper mapper)
    {
        _logger = logger;
        this.mapper = mapper;
    }

    /// <summary>
    /// Get Order Statuses
    /// </summary>
    [HttpGet]
    public async Task<OrderStatuses> GetOrderStatuses(
        [FromServices] WebApi.Hal.Mapper halMapper,
        [FromServices] IRequestClient<GetOrderStatusesQuery> client,
        int skip = 0, int limit = 10, [FromQuery] string[] embed = null!)
        {
            var response = await client.GetResponse<GetOrderStatusesQueryResponse>(new GetOrderStatusesQuery()
            {
                Skip = skip,
                Limit = limit
            });

            var message = response.Message;

            var r = mapper.Map(message.OrderStatuses, embed);

            r.Count = message.OrderStatuses.Count();
            r.Total = message.Total;

            return halMapper.Append("/orderStatuses", r, skip, limit, embed, (skip + r.Count) < r.Total);
        }
}