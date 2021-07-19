using System;
using System.Collections.Generic;
using System.Linq;

using Carts.Contracts;
using Carts.Domain.Entities;

using OrderPriceCalculator;

namespace Carts
{
    public static class CartExt
    {
        public static Dictionary<double, CartTotalDto> Vat(this Cart cart)
        {
            return cart.Items
                    .GroupBy(i => i.VatRate)
                    .Select(g =>
                    {
                        return (
                            VatRate: (g.Key * 100),
                            SubTotal: Math.Round(g.Select(i => i.SubTotal()).Sum(i => i), 2),
                            Vat: Math.Round(g.Select(i => i.Vat()).Sum(i => i), 2),
                            Total: Math.Round(g.Select(i => i.Total()).Sum(i => i), 2));
                    })
                    .ToDictionary(x => x.VatRate, x => new CartTotalDto(x.SubTotal, x.Vat, x.Total));
        }
    }
}