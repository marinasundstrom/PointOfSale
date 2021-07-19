using Carts.Contracts;

using MassTransit;

using Microsoft.AspNetCore.Mvc;

namespace WebApi.Carts;

public partial class CartsController : ControllerBase
{
    [HttpPost("{cartId:guid}/Items")]
    public async Task AddItem([FromServices] IRequestClient<AddCartItemCommand> client,
        Guid cartId, string? description, string? itemId, string? unit, double quantity = 1)
    {
        var r = await client.GetResponse<AddCartItemCommandResponse>(new AddCartItemCommand(cartId, description, itemId, unit, quantity));
    }

    [HttpDelete("{cartId:guid}/Items/{cartItemId}")]
    public async Task RemoveItem([FromServices] IRequestClient<RemoveCartItemCommand> client,
        Guid cartId, Guid cartItemId)
    {
        await client.GetResponse<RemoveCartItemCommandResponse>(new RemoveCartItemCommand(cartId, cartItemId));
    }

    [HttpPut("{cartId:guid}/Items/{cartItemId}/Quantity")]
    public async Task UpdateItemQuantity([FromServices] IRequestClient<UpdateCartItemQuantityCommand> client,
        Guid cartId, Guid cartItemId, [FromBody] double quantity)
    {
        await client.GetResponse<UpdateCartItemQuantityCommandResponse>(new UpdateCartItemQuantityCommand(cartId, cartItemId, quantity));
    }

    [HttpPost("{cartId:guid}/Items/Clear")]
    public async Task ClearCart([FromServices] IRequestClient<ClearCartCommand> client, Guid cartId)
    {
        await client.GetResponse<ClearCartCommandResponse>(new ClearCartCommand(cartId));
    }

    /*

    [HttpGet("{CartNo}")]
    public async Task<WebApi.Carts.Cart> GetCartByTag(
        [FromServices] IRequestClient<GetCartByCartNoQuery> client,
        [FromServices] IRequestClient<GetReceiptsQuery> client2,
        int CartNo, [FromQuery] string[] embed)
    {
        var response = await client.GetResponse<CartDto>(new GetCartByCartNoQuery() {
            CartNo = CartNo,
            IncludeItems = embed.Contains("all") || embed.Contains("items"),
            IncludeDiscounts = embed.Contains("all") || embed.Contains("discounts"),
            IncludeCharges = embed.Contains("all") || embed.Contains("charges"),
        });

        var response2 = await client2.GetResponse<GetReceiptsQueryResponse>(new GetReceiptsQuery() {
            //IncludeItems = embed.Contains("all") || embed.Contains("items"),
            //IncludeDiscounts = embed.Contains("all") || embed.Contains("discounts"),
            //IncludeCharges = embed.Contains("all") || embed.Contains("charges"),
            Filter = response.Message.CartNo.ToString()
        });

        return mapper.Map(response.Message, embed, response2.Message.Receipts.FirstOrDefault());
    }

    */
}