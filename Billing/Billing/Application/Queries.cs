using Billing.Contracts;
using Billing.Domain.Entities;
using Billing.Infrastructure.Persistence;

using Catalog.Client;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Billing.Application;

public class GetReceiptsQueryHandler : IConsumer<GetReceiptsQuery>
{
    private readonly ILogger<GetReceiptsQueryHandler> _logger;
    private readonly BillingContext context;

    public GetReceiptsQueryHandler(
        ILogger<GetReceiptsQueryHandler> logger,
         BillingContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public async Task Consume(ConsumeContext<GetReceiptsQuery> consumeContext)
    {
        var message = consumeContext.Message;

        var query = context.Receipts
            .IncludeAll(
                includeItems: message.IncludeItems,
                includeDiscounts: message.IncludeDiscounts,
                includeCharges: message.IncludeCharges
            )
            .Where(c => message.Filter == null || c.CustomFields.Any(m => m.CustomFieldId == "orderNo" && m.Value == message.Filter))
            .OrderBy(o => o.ReceiptNo)
            .AsSplitQuery()
            .AsNoTracking();

        var total = await query.CountAsync();

        if (message.Limit > 0)
        {
            query = query
                .Skip(message.Skip);
        }

        query = query.Take(message.Limit);

        var receipts = await query.ToArrayAsync();

        var response = new GetReceiptsQueryResponse()
        {
            Receipts = receipts.Select(Mappings.CreateReceiptDto),
            Total = total
        };

        await consumeContext.RespondAsync<GetReceiptsQueryResponse>(response);
    }
}

public class GetReceiptByReceiptNoQueryHandler : IConsumer<GetReceiptByReceiptNoQuery>
{
    private readonly ILogger<GetReceiptByReceiptNoQueryHandler> _logger;
    private readonly BillingContext context;

    public GetReceiptByReceiptNoQueryHandler(
        ILogger<GetReceiptByReceiptNoQueryHandler> logger,
        BillingContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public async Task Consume(ConsumeContext<GetReceiptByReceiptNoQuery> consumeContext)
    {
        var message = consumeContext.Message;

        var order = await context.Receipts
            .IncludeAll(
                includeItems: message.IncludeItems,
                includeDiscounts: message.IncludeDiscounts,
                includeCharges: message.IncludeCharges
            )
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ReceiptNo == message.ReceiptNo);

        if (order is null)
        {
            throw new Exception();
        }

        var dto = Mappings.CreateReceiptDto(order);

        await consumeContext.RespondAsync<ReceiptDto>(dto);
    }
}

public class QueryReceiptsByCustomFieldValueQueryHandler : IConsumer<QueryReceiptsByCustomFieldValueQuery>
{
    private readonly ILogger<QueryReceiptsByCustomFieldValueQueryHandler> _logger;
    private readonly BillingContext context;

    public QueryReceiptsByCustomFieldValueQueryHandler(
        ILogger<QueryReceiptsByCustomFieldValueQueryHandler> logger,
         BillingContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public async Task Consume(ConsumeContext<QueryReceiptsByCustomFieldValueQuery> consumeContext)
    {
        var message = consumeContext.Message;

        var receipts = await context.Receipts
            .AsQueryable()
            .IncludeAll()
            .OrderBy(c => c.Date)
            .Where(c => c.CustomFields.Any(m => m.CustomFieldId == message.CustomFieldId && m.Value == message.Value))
            .AsNoTracking()
            .ToArrayAsync();

        var dtos = receipts.Select(Mappings.CreateReceiptDto);

        await consumeContext.RespondAsync<QueryReceiptsByCustomFieldValueQueryResponse>(new QueryReceiptsByCustomFieldValueQueryResponse
        {
            Receipts = dtos
        });
    }
}