using System.Collections.Generic;

namespace Warehouse.Contracts
{
    public class AdjustWarehouseItemQuantityOnHandCommand
    {
        public string ItemId { get; set; } = null!;

        public int QuantityOnHand { get; set; }
    }

    public class AdjustWarehouseItemQuantityOnHandCommandResponse
    {

    }

    public class ReserveWarehouseItemCommand
    {
        public string ItemId { get; set; } = null!;

        public int QuantityReserved { get; set; }
    }

    public class ReserveWarehouseItemCommandResponse
    {

    }
}