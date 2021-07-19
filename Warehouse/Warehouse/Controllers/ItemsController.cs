using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Warehouse.Contracts;
using Warehouse.Infrastructure.Persistence;

namespace Warehouse.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly ILogger<ItemsController> _logger;
        private readonly WarehouseContext context;

        public ItemsController(ILogger<ItemsController> logger, WarehouseContext context)
        {
            _logger = logger;
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<WarehouseItemDto>> GetItems([FromServices] IRequestClient<GetWarehouseItemsQuery> client)
        {
            var response = await client.GetResponse<GetWarehouseItemsQueryResponse>(new GetWarehouseItemsQuery());

            return response.Message.Items;
        }

        [HttpGet("GetItemsByItemId")]
        public async Task<IEnumerable<WarehouseItemDto>> GetItemsByItemId([FromServices] IRequestClient<GetWarehouseItemsByIdQuery> client, [FromQuery] string[] ids)
        {
            var response = await client.GetResponse<GetWarehouseItemsByIdQueryResponse>(new GetWarehouseItemsByIdQuery() { Ids = ids });

            return response.Message.Items;
        }

        [HttpGet("{id}")]
        public async Task<WarehouseItemDto> GetItemByItemId([FromServices] IRequestClient<GetWarehouseItemByIdQuery> client, string id)
        {
            var response = await client.GetResponse<WarehouseItemDto>(new GetWarehouseItemByIdQuery() { Id = id });

            return response.Message;
        }

        [HttpGet("GetItemByCode")]
        public async Task<WarehouseItemDto> GetItemByCode(string code)
        {
            var warehouseItem = await context.Items
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == code);

            if (warehouseItem is null)
            {
                throw new Exception();
            }

            return Mappings.ToItemDto(warehouseItem);
        }

        [HttpGet("GetItemBySku")]
        public async Task<WarehouseItemDto> GetItemBySku(string sku)
        {
            var warehouseItem = await context.Items
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Sku == sku);

            if (warehouseItem is null)
            {
                throw new Exception();
            }

            return Mappings.ToItemDto(warehouseItem);
        }

        [HttpPut("{id}/QuantityOnHand")]
        public async Task AdjustQuantityOnHand([FromServices] IRequestClient<AdjustWarehouseItemQuantityOnHandCommand> client, string id, int quantityOnHand)
        {
            await client.GetResponse<AdjustWarehouseItemQuantityOnHandCommandResponse>(new AdjustWarehouseItemQuantityOnHandCommand()
            {
                ItemId = id,
                QuantityOnHand = quantityOnHand
            });
        }

        [HttpPost("{id}/ReserveQuantity")]
        public async Task ReserveQuantity([FromServices] IRequestClient<ReserveWarehouseItemCommand> client, string id, int quantityReserved)
        {
            await client.GetResponse<ReserveWarehouseItemCommandResponse>(new ReserveWarehouseItemCommand()
            {
                ItemId = id,
                QuantityReserved = (int)quantityReserved
            });
        }
    }
}