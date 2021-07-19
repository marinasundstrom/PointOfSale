using System;
using System.Collections.Generic;

namespace Carts.Contracts
{
    public class GetCartsQuery
    {
        public int Skip { get; set; }

        public int Limit { get; set; } = 10;

        public bool IncludeItems { get; set; } = true;

        public bool IncludeDiscounts { get; set; } = true;

        public bool IncludeCharges { get; set; } = true;
    }

    public class GetCartsQueryResponse
    {
        public IEnumerable<CartDto> Carts { get; set; } = null!;

        public int Total { get; set; }
    }

    public class GetCartQuery
    {

    }

    public class GetCartByIdQuery
    {
        public Guid Id { get; set; }

        public int? OrderNo { get; set; }

        public bool IncludeItems { get; set; } = true;

        public bool IncludeDiscounts { get; set; } = true;

        public bool IncludeCharges { get; set; } = true;
    }

    public class GetCartByTagQuery
    {
        public string? Tag { get; set; }

        public int? OrderNo { get; set; }

        public bool IncludeItems { get; set; } = true;

        public bool IncludeDiscounts { get; set; } = true;

        public bool IncludeCharges { get; set; } = true;
    }
}