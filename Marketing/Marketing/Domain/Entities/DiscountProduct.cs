using System;

namespace Marketing.Domain.Entities;

public class DiscountProduct
{
    public Guid Id { get; set; }

    public Discount Discount { get; set; } = null!;

    public string ItemId { get; set; } = null!;

    public double Quantity { get; set; }
}