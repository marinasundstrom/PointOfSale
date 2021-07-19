using System;

namespace Warehouse.Contracts
{
    public class WarehouseItemDto
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Sku { get; set; }

        public string? Code { get; set; }
    }
}