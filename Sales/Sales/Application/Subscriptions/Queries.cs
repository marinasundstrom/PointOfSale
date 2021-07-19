using System;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.EntityFrameworkCore;

using Sales.Application.Subscriptions;
using Sales.Contracts;
using Sales.Domain.Entities;
using Sales.Infrastructure.Persistence;

using static Sales.Application.Subscriptions.Mappings;

namespace Sales.Application.Subscriptions
{
    public class GetSubscriptionsQueryHandler : IConsumer<GetSubscriptionsQuery>
    {
        private readonly SalesContext salesContext;

        public GetSubscriptionsQueryHandler(SalesContext salesContext)
        {
            this.salesContext = salesContext;
        }

        public async Task Consume(ConsumeContext<GetSubscriptionsQuery> consumeContext)
        {
            var subscriptions = await salesContext.Subscriptions
                .AsNoTracking()
                .ToListAsync();

            var response = new GetSubscriptionsQueryResponse
            {
                Subscriptions = subscriptions.Select(Map)
            };

            await consumeContext.RespondAsync<GetSubscriptionsQueryResponse>(response);
        }
    }

    public class GetSubscriptionQueryHandler : IConsumer<GetSubscriptionQuery>
    {
        private readonly SalesContext salesContext;

        public GetSubscriptionQueryHandler(SalesContext salesContext)
        {
            this.salesContext = salesContext;
        }

        public async Task Consume(ConsumeContext<GetSubscriptionQuery> consumeContext)
        {
            var request = consumeContext.Message;

            var subscription = await salesContext.Subscriptions
                .FirstOrDefaultAsync(c => c.Id == request.SubscriptionId);

            if (subscription is null)
            {
                throw new Exception();
            }

            await consumeContext.RespondAsync<SubscriptionDto>(Map(subscription));
        }
    }
}