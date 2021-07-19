using System;

using Warehouse.Contracts;
using Warehouse.Domain.Entities;

namespace Warehouse
{
    public static class Mappings
    {
        public static WarehouseItemDto ToItemDto(this WarehouseItem warehouseItem)
        {
            return new WarehouseItemDto()
            {
                Id = warehouseItem.ItemId,
                Name = warehouseItem.Name,
                Description = warehouseItem.Description,
                Sku = warehouseItem.Sku,
                Code = warehouseItem.Code,
            };
        }
    }
}