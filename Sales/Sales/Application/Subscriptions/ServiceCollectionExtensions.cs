﻿using Microsoft.Extensions.DependencyInjection;

namespace Sales.Application.Subscriptions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSubscriptions(this IServiceCollection services)
        {
            services.AddScoped<SubscriptionOrderDateGenerator>()
                    .AddScoped<SubscriptionOrderGenerator>();

            return services;
        }
    }
}