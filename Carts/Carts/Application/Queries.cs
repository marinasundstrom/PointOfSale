using System;
using System.Linq;
using System.Threading.Tasks;

using Carts.Contracts;
using Carts.Domain.Entities;
using Carts.Infrastructure.Persistence;

using Catalog.Client;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Carts.Application
{
    public class GetCartsQueryHandler : IConsumer<GetCartsQuery>
    {
        private readonly ILogger<GetCartsQueryHandler> _logger;
        private readonly CartContext context;
        private readonly IItemsClient catalogItemsClient;
        private readonly IBus bus;

        public GetCartsQueryHandler(
            ILogger<GetCartsQueryHandler> logger,
            CartContext context,
            IItemsClient catalogItemsClient,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.catalogItemsClient = catalogItemsClient;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<GetCartsQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var query = context.Carts
                .IncludeAll() //(includeItems: false, includeSubscriptions: false, includeCustomFields: false)
                .AsNoTracking()
                .AsSplitQuery()
                .AsNoTracking();

            var total = await query.CountAsync();

            var carts = await query
                .Skip(message.Skip)
                .Take(message.Limit)
                .ToArrayAsync();

            var response = new GetCartsQueryResponse()
            {
                Carts = carts.Select(Mappers.CreateCartDto),
                Total = total
            };

            await consumeContext.RespondAsync<GetCartsQueryResponse>(response);
        }
    }

    public class GetCartQueryHandler : IConsumer<GetCartQuery>
    {
        private readonly ILogger<GetCartsQueryHandler> _logger;
        private readonly CartContext context;
        private readonly IItemsClient catalogItemsClient;
        private readonly IBus bus;

        public GetCartQueryHandler(
            ILogger<GetCartsQueryHandler> logger,
            CartContext context,
            IItemsClient catalogItemsClient,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.catalogItemsClient = catalogItemsClient;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<GetCartQuery> consumeContext)
        {
            var cart = await context.Carts
                .IncludeAll()
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (cart is null)
            {
                cart = new Cart();
                context.Carts.Add(cart);
                await context.SaveChangesAsync();
            }

            var dto = Mappers.CreateCartDto(cart);

            await consumeContext.RespondAsync<CartDto>(dto);
        }
    }

    public class GetCartQueryByIdHandler : IConsumer<GetCartByIdQuery>
    {
        private readonly ILogger<GetCartQueryByIdHandler> _logger;
        private readonly CartContext context;
        private readonly IItemsClient catalogItemsClient;
        private readonly IBus bus;

        public GetCartQueryByIdHandler(
            ILogger<GetCartQueryByIdHandler> logger,
            CartContext context,
            IItemsClient catalogItemsClient,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.catalogItemsClient = catalogItemsClient;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<GetCartByIdQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var cart = await context.Carts
                .IncludeAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == message.Id);

            if (cart is null)
            {
                throw new Exception();
            }

            var dto = Mappers.CreateCartDto(cart);

            await consumeContext.RespondAsync<CartDto>(dto);
        }
    }

    public class GetCartQueryByTagHandler : IConsumer<GetCartByTagQuery>
    {
        private readonly ILogger<GetCartQueryByTagHandler> _logger;
        private readonly CartContext context;
        private readonly IItemsClient catalogItemsClient;
        private readonly IBus bus;

        public GetCartQueryByTagHandler(
            ILogger<GetCartQueryByTagHandler> logger,
            CartContext context,
            IItemsClient catalogItemsClient,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.catalogItemsClient = catalogItemsClient;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<GetCartByTagQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var cart = await context.Carts
                .IncludeAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Tag == message.Tag);

            if (cart is null)
            {
                throw new Exception();
            }

            var dto = Mappers.CreateCartDto(cart);

            await consumeContext.RespondAsync<CartDto>(dto);
        }
    }
}