using System;
using System.Collections.Generic;
using System.Linq;

using Sales.Contracts;
using Sales.Domain.Entities;

namespace Sales
{
    public static class OrderExt
    {
        public static Dictionary<string, OrderTotalDto> Vat(this Order order)
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
                   .ToDictionary(x => x.VatRate.ToString(), x => new OrderTotalDto(x.SubTotal, x.Vat, x.Total));
        }
    }
}