using System;
using System.Collections.Generic;

namespace Sales.Contracts
{
    public class UpdateSubscriptionCommand
    {
        public UpdateSubscriptionCommand(Guid subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }

        public Guid SubscriptionId { get; }
    }
}