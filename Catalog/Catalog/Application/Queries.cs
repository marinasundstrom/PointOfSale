using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Catalog.Contracts;
using Catalog.Infrastructure.Persistence;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Catalog.Application
{
    public class GetCatalogItemsQueryHandler : IConsumer<GetCatalogItemsQuery>
    {
        private readonly ILogger<GetCatalogItemsQueryHandler> _logger;
        private readonly CatalogContext context;
        private readonly IDistributedCache cache;

        public GetCatalogItemsQueryHandler(
            ILogger<GetCatalogItemsQueryHandler> logger,
            CatalogContext context,
            IDistributedCache cache)
        {
            _logger = logger;
            this.context = context;
            this.cache = cache;
        }

        public async Task Consume(ConsumeContext<GetCatalogItemsQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var query = context.Items
                    .Include(p => p.Unit)
                    .Include(p => p.VatCodeObj)
                    .Include(p => p.Charges)
                    .Include(p => p.CustomFields)
                    .OrderBy(p => p.ItemId)
                    .AsSplitQuery()
                    .AsNoTracking();

            var total = await query.CountAsync();

            var items = await query
                .Skip(message.Skip)
                .Take(message.Limit)
                .ToArrayAsync();

            var response = new GetCatalogItemsQueryResponse()
            {
                Items = items.Select(Mappings.ToItemDto),
                Total = total
            };

            await consumeContext.RespondAsync<GetCatalogItemsQueryResponse>(response);
        }
    }

    public class GetCatalogItemByIdQueryHandler : IConsumer<GetCatalogItemByIdQuery>
    {
        private readonly ILogger<GetCatalogItemByIdQueryHandler> _logger;
        private readonly CatalogContext context;
        private readonly IDistributedCache cache;

        public GetCatalogItemByIdQueryHandler(
            ILogger<GetCatalogItemByIdQueryHandler> logger,
            CatalogContext context,
            IDistributedCache cache)
        {
            _logger = logger;
            this.context = context;
            this.cache = cache;
        }

        public async Task Consume(ConsumeContext<GetCatalogItemByIdQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var catalogItem = await context.Items
                .Include(p => p.Unit)
                .Include(p => p.VatCodeObj)
                .Include(p => p.Charges)
                .Include(p => p.CustomFields)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ItemId == message.Id);

            if (catalogItem is null)
            {
                throw new Exception();
            }

            var dto = Mappings.ToItemDto(catalogItem);

            await consumeContext.RespondAsync<CatalogItemDto>(dto);
        }
    }

    public class GetCatalogItemsByIdQueryHandler : IConsumer<GetCatalogItemsByIdQuery>
    {
        private readonly ILogger<GetCatalogItemsByIdQueryHandler> _logger;
        private readonly CatalogContext context;
        private readonly IDistributedCache cache;

        public GetCatalogItemsByIdQueryHandler(
            ILogger<GetCatalogItemsByIdQueryHandler> logger,
            CatalogContext context,
            IDistributedCache cache)
        {
            _logger = logger;
            this.context = context;
            this.cache = cache;
        }

        public async Task Consume(ConsumeContext<GetCatalogItemsByIdQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var items = await context.Items
                .Include(p => p.Unit)
                .Include(p => p.VatCodeObj)
                .Include(p => p.Charges)
                .Include(p => p.CustomFields)
                .Where(p => message.Ids.Any(id => p.ItemId == id))
                .AsNoTracking()
                .ToArrayAsync();

            IEnumerable<CatalogItemDto>? dtos = items.Select(Mappings.ToItemDto);

            var response = new GetCatalogItemsByIdQueryResponse()
            {
                Items = dtos
            };

            await consumeContext.RespondAsync<GetCatalogItemsByIdQueryResponse>(response);
        }
    }
}