using System.Collections.Generic;

using Billing.Domain.Entities.Common;

using Microsoft.EntityFrameworkCore;

namespace Billing.Domain.ValueObjects;

[Owned]
public class CurrencyAmount : ValueObject
{
    public string Currency { get; set; } = null!;

    public decimal Amount { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Currency;
        yield return Amount;
    }

    public static CurrencyAmount operator +(CurrencyAmount right, CurrencyAmount left)
    {
        return new CurrencyAmount { Currency = right.Currency, Amount = right.Amount + left.Amount };
    }
}