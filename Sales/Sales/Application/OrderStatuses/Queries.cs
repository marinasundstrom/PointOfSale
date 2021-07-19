using System;
using System.Linq;
using System.Threading.Tasks;

using Catalog.Client;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OrderPriceCalculator;

using Sales.Contracts;
using Sales.Domain.Entities;

using Sales.Infrastructure.Persistence;

namespace Sales.Application.OrderStatuses
{
    public class GetOrderStatusesQueryHandler : IConsumer<GetOrderStatusesQuery>
    {
        private readonly ILogger<GetOrderStatusesQueryHandler> _logger;
        private readonly SalesContext context;

        public GetOrderStatusesQueryHandler(
            ILogger<GetOrderStatusesQueryHandler> logger,
            SalesContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Consume(ConsumeContext<GetOrderStatusesQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var query = context.OrderStatuses
                .AsSplitQuery()
                .AsNoTracking();

            var total = await query.CountAsync();

            var orderStatuses = await query
                .Skip(message.Skip)
                .Take(message.Limit)
                .ToArrayAsync();

            var response = new GetOrderStatusesQueryResponse()
            {
                OrderStatuses = orderStatuses.Select(Mappings.CreateOrderStatusDto),
                Total = total
            };

            await consumeContext.RespondAsync<GetOrderStatusesQueryResponse>(response);
        }
    }
}