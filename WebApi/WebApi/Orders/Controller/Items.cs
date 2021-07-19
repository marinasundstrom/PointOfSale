using Billing.Contracts;

using MassTransit;

using Microsoft.AspNetCore.Mvc;

using Sales.Contracts;

using WebApi.Shared;

namespace WebApi.Orders;

public partial class OrdersController : ControllerBase
{
    [HttpGet("{orderNo:int}/Items/{orderItemId:guid}")]
    public async Task<WebApi.Orders.OrderItem> GetOrderItemById(
        [FromServices] IRequestClient<GetOrderItemQuery> client,
        int orderNo, Guid orderItemId, [FromQuery] string[] embed = null!)
    {
        var response = await client.GetResponse<OrderItemDto>(
            new GetOrderItemQuery()
            {
                OrderNo = orderNo,
                OrderItemId = orderItemId
            });

        return mapper.Map(orderNo, response.Message, embed);
    }
    
    /// <summary>
    /// Add Order Item
    /// </summary>
    [HttpPost("{orderNo:int}/Items")]
    public async Task<OrderItem> AddItem([
        FromServices] IRequestClient<AddOrderItemCommand> client,
        int orderNo,
        AddOrderItemDetails details)
    {
        var response = await client.GetResponse<OrderItemDto>(new AddOrderItemCommand(orderNo, details.Description, details.ItemId, details.Unit, details.Quantity));
        return mapper.Map(orderNo, response.Message, Array.Empty<string>());
    }

    [HttpPut("{orderNo:int}/Items/{orderItemId:guid}/Quantity")]
    public async Task UpdateOrderItemQuantity(
        [FromServices] IRequestClient<UpdateOrderItemQuantityCommand> client,
        int orderNo, Guid orderItemId, int quantity)
    {
        var response = await client.GetResponse<UpdateOrderItemQuantityCommandResponse>(
            new UpdateOrderItemQuantityCommand(orderNo, orderItemId, quantity));
    }

    [HttpDelete("{orderNo:int}/Items/{orderItemId:guid}")]
    public async Task RemoveOrderItem(
        [FromServices] IRequestClient<RemoveOrderItemCommand> client,
        int orderNo, Guid orderItemId)
    {
        var response = await client.GetResponse<RemoveOrderItemCommandResponse>(
            new RemoveOrderItemCommand(orderNo, orderItemId));
    }

    [HttpPost("{orderNo:int}/Items/{orderItemId:guid}/Discounts")]
    public async Task AddDiscountToOrderItem(
        [FromServices] IRequestClient<AddDiscountToOrderItemCommand> client,
        int orderNo, Guid orderItemId, Sales.Contracts.DiscountDetails discount)
    {
        var response = await client.GetResponse<AddDiscountToOrderItemCommandResponse>(
            new AddDiscountToOrderItemCommand(orderNo, orderItemId, discount));
    }

    [HttpDelete("{orderNo:int}/Items/{orderItemId:guid}/Discounts/{discountId:guid}")]
    public async Task RemoveDiscountFromOrderItem(
        [FromServices] IRequestClient<RemoveDiscountFromOrderItemCommand> client,
        int orderNo, Guid orderItemId, Guid discountId)
    {
        var response = await client.GetResponse<RemoveDiscountFromOrderItemCommandResponse>(
            new RemoveDiscountFromOrderItemCommand(orderNo, orderItemId, discountId));
    }

    [HttpPost("{orderNo:int}/Items/{orderItemId:guid}/Charges")]
    public async Task AddChargeToOrderItem(
        [FromServices] IRequestClient<AddChargeToOrderItemCommand> client,
        int orderNo, Guid orderItemId, Sales.Contracts.ChargeDetails charge)
    {
        var response = await client.GetResponse<AddChargeToOrderItemCommandResponse>(
            new AddChargeToOrderItemCommand(orderNo, orderItemId, charge));
    }

    [HttpDelete("{orderNo:int}/Items/{orderItemId:guid}/Charges/{chargeId:guid}")]
    public async Task RemoveChargeFromOrderItem(
        [FromServices] IRequestClient<RemoveChargeFromOrderItemCommand> client,
        int orderNo, Guid orderItemId, Guid chargeId)
    {
        var response = await client.GetResponse<RemoveChargeFromOrderItemCommandResponse>(
            new RemoveChargeFromOrderItemCommand(orderNo, orderItemId, chargeId));
    }

    [HttpPost("{orderNo:int}/{orderItemId:guid}/CustomFields")]
    public async Task AddCustomFieldToOrderItem(
        [FromServices] IRequestClient<AddCustomFieldToOrderItemCommand> client,
        int orderNo, Guid orderItemId, WebApi.Shared.CreateOrUpdateCustomField details)
    {

        var response = await client.GetResponse<AddCustomFieldToOrderItemCommandResponse>(
            new AddCustomFieldToOrderItemCommand(orderNo, orderItemId, new Sales.Contracts.CreateCustomFieldDetails {
                CustomFieldId = details.Name,
                Value = details.Value.ToString()!
            }));
    }

    [HttpDelete("{orderNo:int}/{orderItemId:guid}/CustomFields/{customFieldId}")]
    public async Task RemoveCustomFieldFromOrderItem(
        [FromServices] IRequestClient<RemoveCustomFieldFromOrderItemCommand> client,
        int orderNo, Guid orderItemId, string customFieldId)
    {
        var response = await client.GetResponse<RemoveCustomFieldFromOrderItemCommandResponse>(
            new RemoveCustomFieldFromOrderItemCommand(orderNo, orderItemId, customFieldId));
    }
}