using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

using Payments.Contracts;
using Payments.Infrastructure.Persistence;

namespace Payments.Application
{
    public class GetPaymentsPaymentsQueryHandler : IConsumer<GetPaymentsPaymentsQuery>
    {
        private readonly ILogger<GetPaymentsPaymentsQueryHandler> _logger;
        private readonly PaymentsContext context;
        private readonly IDistributedCache cache;

        public GetPaymentsPaymentsQueryHandler(
            ILogger<GetPaymentsPaymentsQueryHandler> logger,
            PaymentsContext context,
            IDistributedCache cache)
        {
            _logger = logger;
            this.context = context;
            this.cache = cache;
        }

        public async Task Consume(ConsumeContext<GetPaymentsPaymentsQuery> consumeContext)
        {
            //await cache.RemoveAsync("Payments");

            IEnumerable<PaymentDto>? dtos = await cache.GetAsync<IEnumerable<PaymentDto>?>("payments");

            if (dtos is null)
            {
                var payments = await context.Payments
                    .AsNoTracking()
                    .ToArrayAsync();

                dtos = payments.Select(Mappings.ToPaymentDto);

                await cache.SetAsync("payments", dtos, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                });
            }

            var response = new GetPaymentsPaymentsQueryResponse()
            {
                Payments = dtos
            };

            await consumeContext.RespondAsync<GetPaymentsPaymentsQueryResponse>(response);
        }
    }

    public class GetPaymentsByOrderRefQueryHandler : IConsumer<GetPaymentsByOrderRefQuery>
    {
        private readonly ILogger<GetPaymentsByOrderRefQueryHandler> _logger;
        private readonly PaymentsContext context;

        public GetPaymentsByOrderRefQueryHandler(
            ILogger<GetPaymentsByOrderRefQueryHandler> logger,
            PaymentsContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Consume(ConsumeContext<GetPaymentsByOrderRefQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var payments = await context.Payments
                .Where(p => p.OrderRef == message.OrderRef)
                .AsNoTracking()
                .ToArrayAsync();

            IEnumerable<PaymentDto>? dtos = payments.Select(Mappings.ToPaymentDto);

            var response = new GetPaymentsByOrderRefQueryResponse()
            {
                Payments = dtos
            };

            await consumeContext.RespondAsync<GetPaymentsByOrderRefQueryResponse>(response);
        }
    }
}