using Billing.Contracts;
using Billing.Domain.Entities;

using OrderPriceCalculator;

namespace Billing;

public static class ReceiptExt
{
    public static Dictionary<string, ReceiptTotalDto> Vat(this Receipt order)
    {
        return order.Totals
                .Select(g =>
                {
                    return (
                        VatRate: (g.VatRate * 100),
                        SubTotal: Math.Round(g.SubTotal, 2),
                        Vat: Math.Round(g.Vat, 2),
                        Total: Math.Round(g.Total, 2));
                })
                .ToDictionary(x => x.VatRate.ToString(), x => new ReceiptTotalDto(x.SubTotal, x.Vat, x.Total));
    }
}