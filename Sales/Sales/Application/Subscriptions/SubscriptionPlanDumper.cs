using System;

using Sales.Domain.Entities;

namespace Sales.Application.Subscriptions
{
    static class SubscriptionPlanDumper
    {
        public static void Dump(this SubscriptionPlan subscriptionPlan)
        {
            Console.WriteLine(subscriptionPlan.GetDescription());
        }
    }
}