using Billing.Contracts;

using MassTransit;

using Microsoft.AspNetCore.Mvc;

using Sales.Contracts;

using WebApi.Shared;

namespace WebApi.Orders;

[ApiController]
[Route("[controller]")]
[Produces("application/prs.hal-forms+json")]
public partial class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;
    private readonly WebApi.Orders.Mapper mapper;

    public OrdersController(ILogger<OrdersController> logger, WebApi.Orders.Mapper mapper)
    {
        _logger = logger;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<WebApi.Orders.Orders> GetOrders(
        [FromServices] WebApi.Hal.Mapper halMapper,
        [FromServices] IRequestClient<GetOrdersQuery> client,
        int skip = 0, int limit = 10, [FromQuery] string[] embed = null!)
    {
        var response = await client.GetResponse<GetOrdersQueryResponse>(new GetOrdersQuery()
        {
            Skip = skip,
            Limit = limit,
            IncludeItems = embed.Contains("all") || embed.Contains("items"),
            IncludeDiscounts = embed.Contains("all") || embed.Contains("discounts"),
            IncludeCharges = embed.Contains("all") || embed.Contains("charges")
        });

        var message = response.Message;

        var r = mapper.Map(message.Orders, embed);

        r.Count = message.Orders.Count();
        r.Total = message.Total;

        return halMapper.Append("/orders", r, skip, limit, embed, (skip + r.Count) < r.Total);
    }

    [HttpGet("{orderNo:int}")]
    public async Task<WebApi.Orders.Order> GetOrderByNo(
        [FromServices] IRequestClient<GetOrderByOrderNoQuery> client,
        [FromServices] IRequestClient<GetReceiptsQuery> client2,
        int orderNo, [FromQuery] string[] embed)
    {
        var response = await client.GetResponse<OrderDto>(new GetOrderByOrderNoQuery()
        {
            OrderNo = orderNo,
            IncludeItems = embed.Contains("all") || embed.Contains("items"),
            IncludeDiscounts = embed.Contains("all") || embed.Contains("discounts"),
            IncludeCharges = embed.Contains("all") || embed.Contains("charges"),
        });

        var response2 = await client2.GetResponse<GetReceiptsQueryResponse>(new GetReceiptsQuery()
        {
            //IncludeItems = embed.Contains("all") || embed.Contains("items"),
            //IncludeDiscounts = embed.Contains("all") || embed.Contains("discounts"),
            //IncludeCharges = embed.Contains("all") || embed.Contains("charges"),
            Filter = response.Message.OrderNo.ToString()
        });

        return mapper.Map(response.Message, embed, response2.Message.Receipts.FirstOrDefault());
    }

    /// <summary>
    /// Create Order
    /// </summary>
    [HttpPost]
    public async Task<int> CreateOrder(
        [FromServices] IRequestClient<CreateOrderCommand> client, [FromBody] CreateOrderDto? dto)
    {
        var response = await client.GetResponse<CreateOrderCommandResponse>(new CreateOrderCommand(dto));
        return response.Message.OrderNo;
    }

    /// <summary>
    /// Update Order Status
    /// </summary>
    [HttpPut("{orderNo:int}/Status")]
    public async Task UpdateOrderStatus([FromServices] IRequestClient<UpdateOrderStatusCommand> client, int orderNo, string orderStatusId)
    {
        await client.GetResponse<UpdateOrderStatusCommandResponse>(new UpdateOrderStatusCommand(orderNo, orderStatusId));
    }

    [HttpPost("{orderNo:int}/Place")]
    public async Task PlaceOrder(
        [FromServices] IRequestClient<PlaceOrderCommand> client,
        int orderNo)
    {
        await System.Threading.Tasks.Task.CompletedTask;
    }

    [HttpPost("{orderNo:int}/Discounts")]
    public async Task AddDiscountToOrder(
        [FromServices] IRequestClient<AddDiscountToOrderCommand> client,
        int orderNo, Sales.Contracts.DiscountDetails discount)
    {
        var response = await client.GetResponse<AddDiscountToOrderCommandResponse>(
            new AddDiscountToOrderCommand(orderNo, discount));
    }

    [HttpDelete("{orderNo:int}/Discounts/{discountId:guid}")]
    public async Task RemoveDiscountFromOrder(
        [FromServices] IRequestClient<RemoveDiscountFromOrderCommand> client,
        int orderNo, Guid discountId)
    {
        var response = await client.GetResponse<RemoveDiscountFromOrderCommandResponse>(
            new RemoveDiscountFromOrderCommand(orderNo, discountId));
    }

    [HttpPost("{orderNo:int}/Charges")]
    public async Task AddChargeToOrder(
        [FromServices] IRequestClient<AddChargeToOrderCommand> client,
        int orderNo, Sales.Contracts.ChargeDetails charge)
    {
        var response = await client.GetResponse<AddChargeToOrderCommandResponse>(
            new AddChargeToOrderCommand(orderNo, charge));
    }

    [HttpDelete("{orderNo:int}/Charges/{chargeId:guid}")]
    public async Task RemoveChargeFromOrder(
        [FromServices] IRequestClient<RemoveChargeFromOrderCommand> client,
        int orderNo, Guid chargeId)
    {
        var response = await client.GetResponse<RemoveChargeFromOrderCommandResponse>(
            new RemoveChargeFromOrderCommand(orderNo, chargeId));
    }

    [HttpPost("{orderNo:int}/CustomFields")]
    public async Task AddCustomFieldToOrder(
        [FromServices] IRequestClient<AddCustomFieldToOrderCommand> client,
        int orderNo, WebApi.Shared.CreateOrUpdateCustomField details)
    {
        var response = await client.GetResponse<AddCustomFieldToOrderCommandResponse>(
            new AddCustomFieldToOrderCommand(orderNo, new Sales.Contracts.CreateCustomFieldDetails {
                CustomFieldId = details.Name,
                Value = details.Value.ToString()!
            }));
    }

    [HttpDelete("{orderNo:int}/CustomFields/{customFieldId}")]
    public async Task RemoveCustomFieldFromOrder(
        [FromServices] IRequestClient<RemoveCustomFieldFromOrderCommand> client,
        int orderNo, string customFieldId)
    {
        var response = await client.GetResponse<RemoveCustomFieldFromOrderCommandResponse>(
            new RemoveCustomFieldFromOrderCommand(orderNo, customFieldId));
    }
}