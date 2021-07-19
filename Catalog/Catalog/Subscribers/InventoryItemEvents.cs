using System;
using System.Linq;
using System.Threading.Tasks;

using Catalog.Infrastructure.Persistence;

using MassTransit;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using Warehouse.Contracts;

namespace Catalog.Subscribers
{
    public class WarehouseItemQuantityAvailableUpdatedEventHandler : IConsumer<WarehouseItemQuantityAvailableUpdatedEvent>
    {
        private readonly CatalogContext context;
        private readonly IRequestClient<GetWarehouseItemQuantityAvailableQuery> getWarehouseItemQuantityAvailableClient;

        public WarehouseItemQuantityAvailableUpdatedEventHandler(
            CatalogContext context,
            IRequestClient<GetWarehouseItemQuantityAvailableQuery> getWarehouseItemQuantityAvailableClient)
        {
            this.context = context;
            this.getWarehouseItemQuantityAvailableClient = getWarehouseItemQuantityAvailableClient;
        }

        public async Task Consume(ConsumeContext<WarehouseItemQuantityAvailableUpdatedEvent> consumeContext)
        {
            var request = consumeContext.Message;

            var catalogItem = await context.Items
                .FirstOrDefaultAsync(p => p.ItemId == request.ItemId);

            if (catalogItem is not null)
            {
                var response = await getWarehouseItemQuantityAvailableClient.GetResponse<GetWarehouseItemQuantityAvailableQueryResponse>(
                    new GetWarehouseItemQuantityAvailableQuery()
                    {
                        ItemId = request.ItemId
                    }
                );

                catalogItem.QuantityAvailable = response.Message.QuantityAvailable;

                await context.SaveChangesAsync();
            }
        }
    }
}