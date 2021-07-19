using System;
using System.Linq;
using System.Threading.Tasks;

using Catalog.Client;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OrderPriceCalculator;

using Sales.Contracts;
using Sales.Domain.Entities;

using Sales.Infrastructure.Persistence;

namespace Sales.Application.Orders
{
    public class GetOrdersQueryHandler : IConsumer<GetOrdersQuery>
    {
        private readonly ILogger<GetOrdersQueryHandler> _logger;
        private readonly SalesContext context;

        public GetOrdersQueryHandler(
            ILogger<GetOrdersQueryHandler> logger,
            SalesContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Consume(ConsumeContext<GetOrdersQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var query = context.Orders
                .IncludeAll(
                    includeItems: message.IncludeItems,
                    includeDiscounts: message.IncludeDiscounts,
                    includeCharges: message.IncludeCharges
                )
                .OrderBy(o => o.OrderNo)
                .AsSplitQuery()
                .AsNoTracking();

            var total = await query.CountAsync();

            var orders = await query
                .Skip(message.Skip)
                .Take(message.Limit)
                .ToArrayAsync();

            var response = new GetOrdersQueryResponse()
            {
                Orders = orders.Select(Mappings.CreateOrderDto),
                Total = total
            };

            await consumeContext.RespondAsync<GetOrdersQueryResponse>(response);
        }
    }

    public class GetOrderQueryByIdHandler : IConsumer<GetOrderByIdQuery>
    {
        private readonly ILogger<GetOrderQueryByIdHandler> _logger;
        private readonly SalesContext context;

        public GetOrderQueryByIdHandler(
            ILogger<GetOrderQueryByIdHandler> logger,
            SalesContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Consume(ConsumeContext<GetOrderByIdQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var order = await context.Orders
                .IncludeAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == message.Id);

            if (order is null)
            {
                throw new Exception();
            }

            var dto = Mappings.CreateOrderDto(order);

            await consumeContext.RespondAsync<OrderDto>(dto);
        }
    }

    public class GetOrderQueryByOrderNoHandler : IConsumer<GetOrderByOrderNoQuery>
    {
        private readonly ILogger<GetOrderQueryByOrderNoHandler> _logger;
        private readonly SalesContext context;

        public GetOrderQueryByOrderNoHandler(
            ILogger<GetOrderQueryByOrderNoHandler> logger,
            SalesContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Consume(ConsumeContext<GetOrderByOrderNoQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var order = await context.Orders
                .IncludeAll(
                    includeItems: message.IncludeItems,
                    includeDiscounts: message.IncludeDiscounts,
                    includeCharges: message.IncludeCharges
                )
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.OrderNo == message.OrderNo);

            if (order is null)
            {
                throw new Exception();
            }

            var dto = Mappings.CreateOrderDto(order);

            await consumeContext.RespondAsync<OrderDto>(dto);
        }
    }

    public class GetOrderItemsQueryHandler : IConsumer<GetOrderItemsQuery>
    {
        private readonly ILogger<GetOrderItemsQueryHandler> _logger;
        private readonly SalesContext context;

        public GetOrderItemsQueryHandler(
            ILogger<GetOrderItemsQueryHandler> logger,
            SalesContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Consume(ConsumeContext<GetOrderItemsQuery> consumeContext)
        {
            var message = consumeContext.Message;
            var order = await context.Orders
                .IncludeAll()
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            var dto = new GetOrderItemsQueryResponse
            {
                OrderItems = order.Items.Select(Mappings.CreateOrderItemDto)
            };

            await consumeContext.RespondAsync<GetOrderItemsQueryResponse>(dto);
        }
    }

    public class GetOrderItemQueryHandler : IConsumer<GetOrderItemQuery>
    {
        private readonly ILogger<GetOrderItemQueryHandler> _logger;
        private readonly SalesContext context;

        public GetOrderItemQueryHandler(
            ILogger<GetOrderItemQueryHandler> logger,
            SalesContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Consume(ConsumeContext<GetOrderItemQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var order = await context.Orders
                .IncludeAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.OrderNo == message.OrderNo);

            if (order is null)
            {
                order = new Order();
            }

            var item = order.Items.FirstOrDefault(i => i.Id == message.OrderItemId);

            if (item is null)
            {
                throw new Exception();
            }

            var dto = Mappings.CreateOrderItemDto(item);

            await consumeContext.RespondAsync<OrderItemDto>(dto);
        }
    }

    public class GetOrderTotalsQueryHandler : IConsumer<GetOrderTotalsQuery>
    {
        private readonly ILogger<GetOrderTotalsQueryHandler> _logger;
        private readonly SalesContext context;

        public GetOrderTotalsQueryHandler(
            ILogger<GetOrderTotalsQueryHandler> logger,
            SalesContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Consume(ConsumeContext<GetOrderTotalsQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var order = await context.Orders
                .IncludeAll()
                .Where(c => c.OrderNo == message.OrderNo)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            var dto = new OrderTotalsDto()
            {
                Totals = order.Vat(),
                SubTotal = order.Items.Sum(i => i.SubTotal()),
                Vat = order.Items.Sum(i => i.Vat() * (decimal)i.Quantity),
                Discounts = order.Discounts.Select(Mappings.CreateOrderDiscountDto),
                Rounding = order.Rounding(),
                Total = order.Total(true)
            };

            await consumeContext.RespondAsync<OrderTotalsDto>(dto);
        }
    }

    public class QueryOrdersByCustomFieldValueQueryHandler : IConsumer<QueryOrdersByCustomFieldValueQuery>
    {
        private readonly ILogger<QueryOrdersByCustomFieldValueQueryHandler> _logger;
        private readonly SalesContext context;

        public QueryOrdersByCustomFieldValueQueryHandler(
            ILogger<QueryOrdersByCustomFieldValueQueryHandler> logger,
            SalesContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Consume(ConsumeContext<QueryOrdersByCustomFieldValueQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var orders = await context.Orders
                .IncludeAll()
                .Where(c => c.CustomFields.Any(m => m.CustomFieldId == message.CustomFieldId && m.Value == message.Value))
                .AsNoTracking()
                .ToArrayAsync();

            var dtos = orders.Select(Mappings.CreateOrderDto);

            await consumeContext.RespondAsync<QueryOrdersByCustomFieldValueQueryResponse>(new QueryOrdersByCustomFieldValueQueryResponse
            {
                Orders = dtos
            });
        }
    }
}