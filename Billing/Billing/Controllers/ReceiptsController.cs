using Billing.Application;
using Billing.Contracts;
using Billing.Domain.Entities;
using Billing.Infrastructure.Persistence;

using Catalog.Client;

using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Billing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceiptsController : ControllerBase
{
    private readonly ILogger<ReceiptsController> _logger;
    private readonly IBus bus;
    private readonly BillingContext context;
    private readonly IItemsClient catalogItemsClient;
    private readonly ReceiptPrinter receiptPrinter;

    public ReceiptsController(ILogger<ReceiptsController> logger, IBus bus, BillingContext context, IItemsClient catalogItemsClient, ReceiptPrinter receiptPrinter)
    {
        _logger = logger;
        this.bus = bus;
        this.context = context;
        this.catalogItemsClient = catalogItemsClient;
        this.receiptPrinter = receiptPrinter;
    }


    [HttpGet]
    public async Task<Billing.Models.Receipts> GetReceipts(int skip = 0, int limit = 10, [FromQuery] string[] embed = null!)
    {
        var query = context.Receipts
            .AsQueryable()
            .IncludeAll(includeItems: embed.Contains("items"), includeSubscriptions: false, includeCustomFields: true /* embed.Contains("customFields") */)
            .OrderBy(c => c.Date)
            .AsNoTracking();

        var total = await query
            .CountAsync();

        var receipts = await query
            .Skip(skip)
            .Take(limit)
            .ToArrayAsync();

        var r = Billing.Models.Mapper.Map(receipts, embed);

        r.Count = receipts.Count();
        r.Total = total;

        return Billing.Hypermedia.Mapper.Append("/api/receipts", r, skip, limit, embed, (skip + r.Count) < r.Total);
    }

    [HttpGet("{receiptNo}")]
    public async Task<Billing.Models.Receipt> GetReceiptByNo(int receiptNo, [FromQuery] string[] embed)
    {
        var receipt = await context.Receipts
            .AsQueryable()
            .IncludeAll(includeItems: embed.Contains("items"), includeSubscriptions: false, includeCustomFields: true /* embed.Contains("customFields") */)
            .OrderBy(c => c.Date)
            .AsNoTracking()
            .FirstAsync(x => x.ReceiptNo == receiptNo);

        return Billing.Models.Mapper.Map(receipt, embed);
    }

    [HttpGet("GetReceiptById")]
    public async Task<ReceiptDto> GetReceiptById(Guid id)
    {
        var receipt = await context.Receipts
            .AsQueryable()
            .IncludeAll()
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (receipt is null)
        {
            receipt = new Receipt();
        }

        return Mappings.CreateReceiptDto(receipt);
    }

    [HttpGet("{id}/Pdf")]
    public async Task<FileStreamResult> ReceiptToPdf(Guid id)
    {
        var receipt = await context.Receipts
            .Where(r => r.Id == id)
            .AsQueryable()
            .IncludeAll()
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (receipt is null)
        {
            receipt = new Receipt();
        }

        receipt.Totals = receipt.Totals.OrderBy(i => i.VatRate).ToList();

        var stream = await receiptPrinter.PrintAsync(receipt);
        return File(stream, "document/pdf", "receipt.pdf");
    }

    [HttpGet("{id}/Html")]
    public async Task<FileStreamResult> ReceiptToHtml(Guid id)
    {
        var receipt = await context.Receipts
            .Where(r => r.Id == id)
            .AsQueryable()
            .IncludeAll()
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (receipt is null)
        {
            receipt = new Receipt();
        }

        receipt.Totals = receipt.Totals.OrderBy(i => i.VatRate).ToList();

        var stream = await receiptPrinter.PrintAsync(receipt, ReceiptFormat.Html);
        return File(stream, "text/html", "receipt.html");
    }

    [HttpPost("{receiptNo}/SendMail")]
    public async Task SendMail([FromServices] IRequestClient<SendReceiptByEmailCommand> client, int receiptNo)
    {
        var response = await client.GetResponse<SendReceiptByEmailCommandResponse>(new SendReceiptByEmailCommand(receiptNo));
    }

    [HttpPost]
    public async Task<ReceiptDto> CreateReceipt([FromServices] IRequestClient<CreateReceiptCommand> client, CreateReceiptDto dto)
    {
        var response = await client.GetResponse<ReceiptDto>(new CreateReceiptCommand(dto));
        return response.Message;
    }

    [HttpGet("{id}/Items")]
    public async Task<IEnumerable<ReceiptItemDto>> GetItems(Guid id)
    {
        var receipt = await context.Receipts
            .Where(r => r.Id == id)
            .AsQueryable()
            .IncludeAll()
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (receipt is null)
        {
            receipt = new Receipt();
        }

        return receipt.Items.Select(Mappings.CreateReceiptItemDto);
    }

    [HttpGet("QueryReceiptsByCustomField")]
    public async Task<IEnumerable<ReceiptDto>> QueryReceiptsByCustomField([FromServices] IRequestClient<QueryReceiptsByCustomFieldValueQuery> client, [FromQuery] string customFieldId, [FromQuery] string? value)
    {
        var r = await client.GetResponse<QueryReceiptsByCustomFieldValueQueryResponse>(new QueryReceiptsByCustomFieldValueQuery()
        {
            CustomFieldId = customFieldId,
            Value = value
        });
        return r.Message.Receipts;
    }
}