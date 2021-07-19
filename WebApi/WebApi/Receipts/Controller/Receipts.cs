using Billing.Contracts;

using MassTransit;

using Microsoft.AspNetCore.Mvc;

using Sales.Contracts;

namespace WebApi.Receipts;

[ApiController]
[Route("[controller]")]
[Produces("application/prs.hal-forms+json")]
public partial class ReceiptsController : ControllerBase
{
    private readonly ILogger<ReceiptsController> _logger;
    private readonly WebApi.Receipts.Mapper mapper;

    public ReceiptsController(ILogger<ReceiptsController> logger, WebApi.Receipts.Mapper mapper)
    {
        _logger = logger;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<WebApi.Receipts.Receipts> GetReceipts(
        [FromServices] WebApi.Hal.Mapper halMapper,
        [FromServices] IRequestClient<GetReceiptsQuery> client,
        int skip = 0, int limit = 10, [FromQuery] string[] embed = null!,
        [FromQuery] string? filter = null)
    {
        var response = await client.GetResponse<GetReceiptsQueryResponse>(new GetReceiptsQuery()
        {
            Skip = skip,
            Limit = limit,
            IncludeItems = embed.Contains("all") || embed.Contains("items"),
            IncludeDiscounts = embed.Contains("all") || embed.Contains("discounts"),
            IncludeCharges = embed.Contains("all") || embed.Contains("charges"),
            Filter = filter
        });

        var message = response.Message;

        var r = mapper.Map(message.Receipts, embed);

        r.Count = message.Receipts.Count();
        r.Total = message.Total;

        return halMapper.Append("/receipts", r, skip, limit, embed, (skip + r.Count) < r.Total);
    }

    [HttpGet("{receiptNo}")]
    public async Task<WebApi.Receipts.Receipt> GetReceiptByNo(
        [FromServices] IRequestClient<GetReceiptByReceiptNoQuery> client,
        int receiptNo, [FromQuery] string[] embed)
    {
        var response = await client.GetResponse<ReceiptDto>(new GetReceiptByReceiptNoQuery()
        {
            ReceiptNo = receiptNo,
            IncludeItems = embed.Contains("all") || embed.Contains("items"),
            IncludeDiscounts = embed.Contains("all") || embed.Contains("discounts"),
            IncludeCharges = embed.Contains("all") || embed.Contains("charges"),
        });

        return mapper.Map(response.Message, embed);
    }

    [HttpGet("{receiptNo}/Pdf")]
    public Task<FileStreamResult> GetReceiptAsPdf(
        [FromServices] IRequestClient<GetReceiptByReceiptNoQuery> client,
        int receiptNo)
    {
        using var stream = new MemoryStream();
        return Task.FromResult(File(stream, "document/pdf", $"{receiptNo}.pdf"));
    }

    [HttpGet("{receiptNo}/Html")]
    public Task<FileStreamResult> GetReceiptAsHtml(
        [FromServices] IRequestClient<GetReceiptByReceiptNoQuery> client,
        int receiptNo)
    {
        using var stream = new MemoryStream();
        return Task.FromResult(File(stream, "text/html", $"{receiptNo}.html"));
    }

    [HttpPost("{receiptNo}/Refund")]
    public Task MakeRefundOnReceipt(
        [FromServices] IRequestClient<GetReceiptByReceiptNoQuery> client,
        int receiptNo)
    {
        return Task.CompletedTask;
    }
}