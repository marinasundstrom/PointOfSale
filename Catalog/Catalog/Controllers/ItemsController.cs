using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Catalog.Contracts;
using Catalog.Infrastructure.Persistence;

using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly ILogger<ItemsController> _logger;
        private readonly CatalogContext context;

        public ItemsController(ILogger<ItemsController> logger, CatalogContext context)
        {
            _logger = logger;
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<CatalogItemDto>> GetItems([FromServices] IRequestClient<GetCatalogItemsQuery> client)
        {
            var response = await client.GetResponse<GetCatalogItemsQueryResponse>(new GetCatalogItemsQuery());

            return response.Message.Items;
        }

        [HttpGet("GetItemsByItemId")]
        public async Task<IEnumerable<CatalogItemDto>> GetItemsByItemId([FromServices] IRequestClient<GetCatalogItemsByIdQuery> client, [FromQuery] string[] ids)
        {
            var response = await client.GetResponse<GetCatalogItemsByIdQueryResponse>(new GetCatalogItemsByIdQuery() { Ids = ids });

            return response.Message.Items;
        }

        [HttpGet("{id}")]
        public async Task<CatalogItemDto> GetItemByItemId([FromServices] IRequestClient<GetCatalogItemByIdQuery> client, string id)
        {
            var response = await client.GetResponse<CatalogItemDto>(new GetCatalogItemByIdQuery() { Id = id });

            return response.Message;
        }

        [HttpGet("GetItemByCode")]
        public async Task<CatalogItemDto> GetItemByCode(string code)
        {
            var catalogItem = await context.Items
                .Include(p => p.Unit)
                .Include(p => p.VatCodeObj)
                .Include(p => p.Charges)
                .Include(p => p.CustomFields)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == code);

            if (catalogItem is null)
            {
                throw new Exception();
            }

            return Mappings.ToItemDto(catalogItem);
        }

        [HttpGet("GetItemBySku")]
        public async Task<CatalogItemDto> GetItemBySku(string sku)
        {
            var catalogItem = await context.Items
                .Include(p => p.Unit)
                .Include(p => p.VatCodeObj)
                .Include(p => p.Charges)
                .Include(p => p.CustomFields)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Sku == sku);

            if (catalogItem is null)
            {
                throw new Exception();
            }

            return Mappings.ToItemDto(catalogItem);
        }

        /// <summary>
        /// Add Custom Field to Item
        /// </summary>
        [HttpPost("{id}/CustomFields")]
        public async Task AddCustomFieldToItem(
            [FromServices] IRequestClient<AddCustomFieldToItemCommand> client, 
            string id, [FromBody] CreateCustomFieldDetails details)
        {
            await client.GetResponse<AddCustomFieldToItemCommandResponse>(
                new AddCustomFieldToItemCommand(id, details)
            );
        }

        /// <summary>
        /// Remove Custom Field from Item
        /// </summary>
        [HttpDelete("{id}/CustomFields/{customFieldId}")]
        public async Task RemoveCustomFieldFromItem(
            [FromServices] IRequestClient<RemoveCustomFieldFromItemCommand> client,
            string id, string customFieldId)
        {
            await client.GetResponse<RemoveCustomFieldFromItemCommandResponse>(new RemoveCustomFieldFromItemCommand(id, customFieldId));
        }
    }
}