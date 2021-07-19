using System;
using System.Linq;
using System.Threading.Tasks;

using Checkout.Contracts;
using Checkout.Hubs;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

using Warehouse.Contracts;


namespace Checkout.Subscribers
{
    public class WarehouseEventsHandler : IConsumer<WarehouseItemQuantityAvailableUpdatedEvent>
    {
        private readonly IHubContext<CatalogHub, ICatalogClient> catalogHubContext;

        public WarehouseEventsHandler(
            IHubContext<CatalogHub, ICatalogClient> catalogHubContext)
        {
            this.catalogHubContext = catalogHubContext;
        }

        public async Task Consume(ConsumeContext<WarehouseItemQuantityAvailableUpdatedEvent> consumeContext)
        {
            var message = consumeContext.Message;

            await catalogHubContext.Clients.All.CatalogItemUpdated(new CatalogItemEvent { ItemId = message.ItemId });
        }
    }
}