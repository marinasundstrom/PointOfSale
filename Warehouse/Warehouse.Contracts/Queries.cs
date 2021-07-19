using System.Collections.Generic;

namespace Warehouse.Contracts
{
    public class GetWarehouseItemsQuery
    {
    }

    public class GetWarehouseItemsQueryResponse
    {
        public IEnumerable<WarehouseItemDto> Items { get; set; } = null!;
    }


    public class GetWarehouseItemByIdQuery
    {
        public string Id { get; set; } = null!;
    }

    public class GetWarehouseItemsByIdQuery
    {
        public IEnumerable<string> Ids { get; set; } = null!;
    }

    public class GetWarehouseItemsByIdQueryResponse
    {
        public IEnumerable<WarehouseItemDto> Items { get; set; } = null!;
    }

    public class GetWarehouseItemQuantityAvailableQuery
    {
        public string ItemId { get; set; } = null!;
    }

    public class GetWarehouseItemQuantityAvailableQueryResponse
    {
        public int? QuantityAvailable { get; set; }
    }
}