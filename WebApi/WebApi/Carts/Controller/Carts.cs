using Carts.Contracts;

using MassTransit;

using Microsoft.AspNetCore.Mvc;

namespace WebApi.Carts;

[ApiController]
[Route("[controller]")]
[Produces("application/prs.hal-forms+json")]
public partial class CartsController : ControllerBase
{
    private readonly ILogger<CartsController> _logger;
    private readonly WebApi.Carts.Mapper mapper;

    public CartsController(ILogger<CartsController> logger, WebApi.Carts.Mapper mapper)
    {
        _logger = logger;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<WebApi.Carts.Carts> GetCarts(
        [FromServices] WebApi.Hal.Mapper halMapper,
        [FromServices] IRequestClient<GetCartsQuery> client,
        int skip = 0, int limit = 10, [FromQuery] string[] embed = null!)
    {
        var response = await client.GetResponse<GetCartsQueryResponse>(new GetCartsQuery()
        {
            Skip = skip,
            Limit = limit,
            IncludeItems = embed.Contains("all") || embed.Contains("items"),
            IncludeDiscounts = embed.Contains("all") || embed.Contains("discounts"),
            IncludeCharges = embed.Contains("all") || embed.Contains("charges")
        });

        var message = response.Message;

        var r = mapper.Map(message.Carts, embed);

        r.Count = message.Carts.Count();
        r.Total = message.Total;

        return halMapper.Append("/carts", r, skip, limit, embed, (skip + r.Count) < r.Total);
    }

    [HttpGet("{cartId:guid}")]
    public async Task<WebApi.Carts.Cart> GetCartById(
        [FromServices] IRequestClient<GetCartByIdQuery> client,
        Guid cartId, [FromQuery] string[] embed)
    {
        var response = await client.GetResponse<CartDto>(new GetCartByIdQuery()
        {
            Id = cartId,
            IncludeItems = embed.Contains("all") || embed.Contains("items"),
            IncludeDiscounts = embed.Contains("all") || embed.Contains("discounts"),
            IncludeCharges = embed.Contains("all") || embed.Contains("charges")
        });

        var message = response.Message;

        return mapper.Map(message, embed);
    }
}