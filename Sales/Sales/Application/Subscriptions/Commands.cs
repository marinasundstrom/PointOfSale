using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.EntityFrameworkCore;

using Sales.Contracts;
using Sales.Domain.Entities;
using Sales.Infrastructure.Persistence;

using static Sales.Application.Subscriptions.Mappings;

namespace Sales.Application.Subscriptions
{
    public class UpdateSubscriptionCommandHandler : IConsumer<UpdateSubscriptionCommand>
    {
        private readonly SalesContext salesContext;

        public UpdateSubscriptionCommandHandler(SalesContext salesContext)
        {
            this.salesContext = salesContext;
        }

        public async Task Consume(ConsumeContext<UpdateSubscriptionCommand> consumeContext)
        {
            var request = consumeContext.Message;

            var subscription = await salesContext.Subscriptions
                .FirstOrDefaultAsync(c => c.Id == request.SubscriptionId);

            if (subscription is null)
            {
                throw new System.Exception();
            }

            // TODO: Update

            await salesContext.SaveChangesAsync();
        }
    }
}