using System;

using Marketing.Domain.Common;

namespace Marketing.Domain.Entities;

public class Discount : AuditableEntity, ISoftDelete
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }


    public string? Code { get; set; }

    public Prospect? Prospect { get; set; }


    //public DiscountType DiscountType  { get; set; }

    //public TargetType Target { get; set; }


    public string? ItemId { get; set; }

    public int? Quantity { get; set; }


    public decimal? Price { get; set; }

    public decimal? Amount { get; set; }

    public double? Percent { get; set; }


    public string? OtherItemId { get; set; }


    //public IEnumerable<DiscountProduct>? Products { get; set; }


    public DateTime? Deleted { get; set; }

    public string? DeletedBy { get; set; }
}