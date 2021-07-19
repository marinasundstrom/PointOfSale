using System;
using System.Linq;
using System.Threading.Tasks;

using Marketing.Contracts;
using Marketing.Domain.Entities;
using Marketing.Infrastructure.Persistence;

using MassTransit;

using Microsoft.EntityFrameworkCore;

namespace Marketing.Application;

public class GetDiscountsQueryHandler : IConsumer<GetDiscountsQuery>
{
    private readonly ILogger<GetDiscountsQueryHandler> _logger;
    private readonly MarketingContext context;

    public GetDiscountsQueryHandler(
        ILogger<GetDiscountsQueryHandler> logger,
        MarketingContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public async Task Consume(ConsumeContext<GetDiscountsQuery> consumeContext)
    {
        var message = consumeContext.Message;

        var discounts = await context.Discounts
            .AsNoTracking()
            .ToArrayAsync();

        await consumeContext.RespondAsync<GetDiscountsQueryResponse>(
            new GetDiscountsQueryResponse()
            {
                Discounts = discounts.Select(ToDiscountsDto)
            }
        );
    }

    private static DiscountDto ToDiscountsDto(Domain.Entities.Discount discount)
    {
        return new DiscountDto()
        {
            Id = discount.Id,
            Name = discount.Name,
            Description = discount.Description,

            ItemId = discount.ItemId,
            Quantity = discount.Quantity,

            OtherItemId = discount.OtherItemId,

            Price = discount.Price,
            Amount = discount.Amount,
            Percent = discount.Percent
        };
    }
}