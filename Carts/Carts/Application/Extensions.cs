using System;
using System.Linq;
using System.Threading.Tasks;

using Carts.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Carts.Application
{
    public static class Extensions
    {
        public static IQueryable<Cart> IncludeAll(this IQueryable<Cart> carts,
            bool includeItems = true,
            bool includeDiscounts = true,
            bool includeCharges = true,
            bool includeSubscriptions = true,
            bool includeCustomFields = true)
        {
            var query = carts
                //.Include(c => c.Status)
                //.Include(c => c.Totals)
                .AsQueryable();

            if (includeCharges)
            {
                query = query
                    .Include(c => c.Charges);
            }

            if (includeDiscounts)
            {
                query = query
                    .Include(c => c.Discounts);
            }

            /*

            if(includeSubscriptions) 
            {
                query = query
                    .Include(c => c.Subscription)
                    .ThenInclude(c => c!.SubscriptionPlan);
            }

            if(includeCustomFields) 
            {
                query = query
                    .Include(c => c.CustomFields);  
            }
            
            */

            if (includeItems)
            {
                query = query.Include(c => c.Items);

                if (includeCharges)
                {
                    query = query
                        .Include(c => c.Items)
                        .ThenInclude(c => c.Charges);
                }

                if (includeDiscounts)
                {
                    query = query
                        .Include(c => c.Items)
                        .ThenInclude(c => c.Discounts);
                }

                /*

                if(includeSubscriptions) 
                {
                    query = query
                        .Include(c => c.Items)
                        .ThenInclude(c => c.Subscription)
                        .ThenInclude(c => c!.SubscriptionPlan);  
                }

                if(includeCustomFields) 
                {
                    query = query
                        .Include(c => c.Items)
                        .ThenInclude(c => c.CustomFields);
                }

                */
            }

            return query;
        }
    }
}