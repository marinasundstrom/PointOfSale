﻿using Microsoft.Extensions.DependencyInjection;

namespace Marketing.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //services.AddMediatR(typeof(ServiceCollectionExtensions));

        return services;
    }
}