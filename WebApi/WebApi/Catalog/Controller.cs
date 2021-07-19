using MassTransit;

using Microsoft.AspNetCore.Mvc;

using global::Catalog.Contracts;

namespace WebApi.Catalog;

[ApiController]
[Route("Catalog/Items")]
[Produces("application/prs.hal-forms+json")]
public class CatalogItemsController : ControllerBase
{
    private readonly ILogger<CatalogItemsController> _logger;
    private readonly WebApi.Catalog.Mapper mapper;

    public CatalogItemsController(ILogger<CatalogItemsController> logger, WebApi.Catalog.Mapper mapper)
    {
        _logger = logger;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<WebApi.Catalog.Items> GetItems(
        [FromServices] WebApi.Hal.Mapper halMapper,
        [FromServices] IRequestClient<GetCatalogItemsQuery> client,
        int skip = 0, int limit = 10, [FromQuery] string[] embed = null!)
    {
        var response = await client.GetResponse<GetCatalogItemsQueryResponse>(new GetCatalogItemsQuery()
        {
            Skip = skip,
            Limit = limit,
            IncludeCharges = embed.Contains("all") || embed.Contains("charges")
        });

        var message = response.Message;

        var r = mapper.Map(message.Items, embed);

        r.Count = message.Items.Count();
        r.Total = message.Total;

        return halMapper.Append("/catalog/items", r, skip, limit, embed, (skip + r.Count) < r.Total);
    }

    [HttpGet("{itemId}")]
    public async Task<WebApi.Catalog.Item> GetItemById(
        [FromServices] IRequestClient<GetCatalogItemByIdQuery> client,
        string itemId, [FromQuery] string[] embed)
    {
        var response = await client.GetResponse<CatalogItemDto>(new GetCatalogItemByIdQuery()
        {
            Id = itemId,
            IncludeCharges = embed.Contains("all") || embed.Contains("charges")
        });

        return mapper.Map(response.Message, embed);
    }

     /// <summary>
    /// Add Custom Field to Item
    /// </summary>
    [HttpPost("{itemId}/CustomFields")]
    public async Task AddCustomFieldToItem(
        [FromServices] IRequestClient<AddCustomFieldToItemCommand> client, 
        string itemId, [FromBody] WebApi.Shared.CreateOrUpdateCustomField details)
    {
        await client.GetResponse<AddCustomFieldToItemCommandResponse>(
            new AddCustomFieldToItemCommand(itemId, new CreateCustomFieldDetails {
                CustomFieldId = details.Name,
                Value = details.Value.ToString()!
            })
        );
    }

    /// <summary>
    /// Remove Custom Field from Item
    /// </summary>
    [HttpDelete("{itemId}/CustomFields/{customFieldId}")]
    public async Task RemoveCustomFieldFromItem(
        [FromServices] IRequestClient<RemoveCustomFieldFromItemCommand> client,
        string itemId, string customFieldId)
    {
        await client.GetResponse<RemoveCustomFieldFromItemCommandResponse>(new RemoveCustomFieldFromItemCommand(itemId, customFieldId));
    }
}