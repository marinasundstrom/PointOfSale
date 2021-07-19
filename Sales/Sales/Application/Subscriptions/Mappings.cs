using System.Collections.Generic;
using System.Linq;

using Sales.Application.Subscriptions;
using Sales.Contracts;
using Sales.Domain.Entities;

namespace Sales.Application.Subscriptions
{
    public static class Mappings
    {
        public static SubscriptionDto Map(Subscription subscription)
        {
            return new SubscriptionDto
            {
                Id = subscription.Id
            };
        }
    }
}