namespace Billing.Contracts;

using System.Collections.Generic;

public class GetReceiptsQuery
{
    public int Skip { get; set; }

    public int Limit { get; set; } = 10;

    public bool IncludeItems { get; set; } = true;

    public bool IncludeDiscounts { get; set; } = true;

    public bool IncludeCharges { get; set; } = true;

    public string? Filter { get; set; }
}

public class GetReceiptsQueryResponse
{
    public IEnumerable<ReceiptDto> Receipts { get; set; } = null!;

    public int Total { get; set; }
}

public class GetReceiptByReceiptNoQuery
{
    public int ReceiptNo { get; set; }

    public bool IncludeItems { get; set; } = true;

    public bool IncludeDiscounts { get; set; } = true;

    public bool IncludeCharges { get; set; } = true;
}

public class QueryReceiptsByCustomFieldValueQuery
{
    public string CustomFieldId { get; set; } = null!;

    public string? Value { get; set; }
}

public class QueryReceiptsByCustomFieldValueQueryResponse
{
    public IEnumerable<ReceiptDto> Receipts { get; set; } = null!;
}