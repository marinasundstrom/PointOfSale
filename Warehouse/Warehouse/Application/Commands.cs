using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

using Warehouse.Contracts;
using Warehouse.Infrastructure.Persistence;

namespace Warehouse.Application
{
    public class AdjustWarehouseItemQuantityOnHandCommandHandler : IConsumer<AdjustWarehouseItemQuantityOnHandCommand>
    {
        private readonly ILogger<AdjustWarehouseItemQuantityOnHandCommandHandler> _logger;
        private readonly WarehouseContext context;
        private readonly IBus bus;

        public AdjustWarehouseItemQuantityOnHandCommandHandler(
            ILogger<AdjustWarehouseItemQuantityOnHandCommandHandler> logger,
            WarehouseContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<AdjustWarehouseItemQuantityOnHandCommand> consumeContext)
        {
            var request = consumeContext.Message;

            var warehouseItem = await context.Items
                .FirstOrDefaultAsync(p => p.ItemId == request.ItemId);

            if (warehouseItem is null)
            {
                throw new Exception();
            }

            if (warehouseItem.QuantityOnHand != request.QuantityOnHand)
            {
                warehouseItem.AdjustQuantityOnHand(request.QuantityOnHand);

                await context.SaveChangesAsync();
            }

            var response = new AdjustWarehouseItemQuantityOnHandCommandResponse();

            await consumeContext.RespondAsync<AdjustWarehouseItemQuantityOnHandCommandResponse>(response);
        }
    }

    public class ReserveWarehouseItemCommandHandler : IConsumer<ReserveWarehouseItemCommand>
    {
        private readonly ILogger<ReserveWarehouseItemCommandHandler> _logger;
        private readonly WarehouseContext context;
        private readonly IBus bus;

        public ReserveWarehouseItemCommandHandler(
            ILogger<ReserveWarehouseItemCommandHandler> logger,
            WarehouseContext context,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<ReserveWarehouseItemCommand> consumeContext)
        {
            var request = consumeContext.Message;

            var warehouseItem = await context.Items
                .FirstOrDefaultAsync(p => p.ItemId == request.ItemId);

            if (warehouseItem is not null)
            {
                warehouseItem.ReserveQuantity(request.QuantityReserved);

                await context.SaveChangesAsync();
            }

            var response = new ReserveWarehouseItemCommandResponse();

            await consumeContext.RespondAsync<ReserveWarehouseItemCommandResponse>(response);
        }
    }
}