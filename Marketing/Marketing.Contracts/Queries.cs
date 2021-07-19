namespace Marketing.Contracts;

using System.Collections.Generic;

public class GetDiscountsQuery
{

}

public class GetDiscountsQueryResponse
{
    public IEnumerable<DiscountDto> Discounts { get; set; } = null!;
}