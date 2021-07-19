using System;

namespace Marketing.Contracts;

public class DiscountDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }


    public string? ItemId { get; set; }

    public int? Quantity { get; set; }


    public string? OtherItemId { get; set; }


    public decimal? Price { get; set; }

    public decimal? Amount { get; set; }

    public double? Percent { get; set; }
}