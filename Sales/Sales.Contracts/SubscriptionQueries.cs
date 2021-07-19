using System;
using System.Collections.Generic;

namespace Sales.Contracts
{
    public class GetSubscriptionsQuery
    {

    }

    public class GetSubscriptionsQueryResponse
    {
        public IEnumerable<SubscriptionDto> Subscriptions { get; set; } = null!;
    }

    public class GetSubscriptionQuery
    {
        public GetSubscriptionQuery(Guid subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }

        public Guid SubscriptionId { get; }
    }
}