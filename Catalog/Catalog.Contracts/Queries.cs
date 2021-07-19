using System.Collections.Generic;

namespace Catalog.Contracts
{
    public class GetCatalogItemsQuery
    {
        public int Skip { get; set; }

        public int Limit { get; set; } = 10;

        public bool IncludeCharges { get; set; } = true;

        public bool IncludeCustomFields { get; set; } = true;
    }

    public class GetCatalogItemsQueryResponse
    {
        public IEnumerable<CatalogItemDto> Items { get; set; } = null!;

        public int Total { get; set; }
    }

    public class GetCatalogItemByIdQuery
    {
        public string Id { get; set; } = null!;

        public bool IncludeCharges { get; set; } = true;
    }

    public class GetCatalogItemsByIdQuery
    {
        public IEnumerable<string> Ids { get; set; } = null!;
    }

    public class GetCatalogItemsByIdQueryResponse
    {
        public IEnumerable<CatalogItemDto> Items { get; set; } = null!;
    }
}