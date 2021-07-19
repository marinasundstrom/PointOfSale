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
    public class GetWarehouseItemsQueryHandler : IConsumer<GetWarehouseItemsQuery>
    {
        private readonly ILogger<GetWarehouseItemsQueryHandler> _logger;
        private readonly WarehouseContext context;
        private readonly IDistributedCache cache;

        public GetWarehouseItemsQueryHandler(
            ILogger<GetWarehouseItemsQueryHandler> logger,
            WarehouseContext context,
            IDistributedCache cache)
        {
            _logger = logger;
            this.context = context;
            this.cache = cache;
        }

        public async Task Consume(ConsumeContext<GetWarehouseItemsQuery> consumeContext)
        {
            //await cache.RemoveAsync("Items");

            IEnumerable<WarehouseItemDto>? dtos = await cache.GetAsync<IEnumerable<WarehouseItemDto>?>("items");

            if (dtos is null)
            {
                var items = await context.Items
                    .AsNoTracking()
                    .ToArrayAsync();

                dtos = items.Select(Mappings.ToItemDto);

                await cache.SetAsync("items", dtos, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                });
            }

            var response = new GetWarehouseItemsQueryResponse()
            {
                Items = dtos
            };

            await consumeContext.RespondAsync<GetWarehouseItemsQueryResponse>(response);
        }
    }

    public class GetWarehouseItemByIdQueryHandler : IConsumer<GetWarehouseItemByIdQuery>
    {
        private readonly ILogger<GetWarehouseItemByIdQueryHandler> _logger;
        private readonly WarehouseContext context;
        private readonly IDistributedCache cache;

        public GetWarehouseItemByIdQueryHandler(
            ILogger<GetWarehouseItemByIdQueryHandler> logger,
            WarehouseContext context,
            IDistributedCache cache)
        {
            _logger = logger;
            this.context = context;
            this.cache = cache;
        }

        public async Task Consume(ConsumeContext<GetWarehouseItemByIdQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var warehouseItem = await context.Items
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ItemId == message.Id);

            if (warehouseItem is null)
            {
                throw new Exception();
            }

            var dto = Mappings.ToItemDto(warehouseItem);

            await consumeContext.RespondAsync<WarehouseItemDto>(dto);
        }
    }

    public class GetWarehouseItemsByIdQueryHandler : IConsumer<GetWarehouseItemsByIdQuery>
    {
        private readonly ILogger<GetWarehouseItemsByIdQueryHandler> _logger;
        private readonly WarehouseContext context;
        private readonly IDistributedCache cache;

        public GetWarehouseItemsByIdQueryHandler(
            ILogger<GetWarehouseItemsByIdQueryHandler> logger,
            WarehouseContext context,
            IDistributedCache cache)
        {
            _logger = logger;
            this.context = context;
            this.cache = cache;
        }

        public async Task Consume(ConsumeContext<GetWarehouseItemsByIdQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var items = await context.Items
                .Where(p => message.Ids.Any(id => p.ItemId == id))
                .AsNoTracking()
                .ToArrayAsync();

            IEnumerable<WarehouseItemDto>? dtos = items.Select(Mappings.ToItemDto);

            var response = new GetWarehouseItemsByIdQueryResponse()
            {
                Items = dtos
            };

            await consumeContext.RespondAsync<GetWarehouseItemsByIdQueryResponse>(response);
        }
    }

    public class GetWarehouseItemQuantityAvailableQueryHandler : IConsumer<GetWarehouseItemQuantityAvailableQuery>
    {
        private readonly ILogger<GetWarehouseItemQuantityAvailableQueryHandler> _logger;
        private readonly WarehouseContext context;
        private readonly IDistributedCache cache;

        public GetWarehouseItemQuantityAvailableQueryHandler(
            ILogger<GetWarehouseItemQuantityAvailableQueryHandler> logger,
            WarehouseContext context,
            IDistributedCache cache)
        {
            _logger = logger;
            this.context = context;
            this.cache = cache;
        }

        public async Task Consume(ConsumeContext<GetWarehouseItemQuantityAvailableQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var item = await context.Items
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.ItemId == message.ItemId);

            if (item == null)
            {
                throw new Exception();
            }

            var response = new GetWarehouseItemQuantityAvailableQueryResponse()
            {
                QuantityAvailable = item.QuantityAvailable
            };

            await consumeContext.RespondAsync<GetWarehouseItemQuantityAvailableQueryResponse>(response);
        }
    }
}