using Marketing.Contracts;
using Marketing.Infrastructure.Persistence;

using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Marketing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiscountsController : ControllerBase
{
    private readonly ILogger<DiscountsController> _logger;

    public DiscountsController(ILogger<DiscountsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<DiscountDto>> GetDiscounts([FromServices] IRequestClient<GetDiscountsQuery> client)
    {
        var response = await client.GetResponse<GetDiscountsQueryResponse>(new GetDiscountsQuery());
        return response.Message.Discounts;
    }
}