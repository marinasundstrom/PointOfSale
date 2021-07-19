using System;
using System.Linq;

using Carts.Contracts;
using Carts.Domain.Entities;

using OrderPriceCalculator;

namespace Carts
{
    public static class Mappers
    {
        public static CartDto CreateCartDto(this Cart cart)
        {
            var dto = new CartDto()
            {
                Id = cart.Id,
                Tag = cart.Tag,
                Items = cart.Items.OrderBy(i => i.Created).Select(CreateCartItemDto),
                Totals = cart.Vat(),
                SubTotal = Math.Round(cart.SubTotal(), 2),
                VatRate = null,
                Vat = Math.Round(((IOrder)cart).Vat(), 2),
                Charges = cart.Charges.Select(CreateCartChargeDto),
                Charge = cart.Charge(),
                Discounts = cart.Discounts.Select(CreateCartDiscountDto),
                Discount = cart.Discount(),
                Rounding = cart.Rounding(),
                Total = cart.Total(true)
            };

            return dto;
        }

        public static CartItemDto CreateCartItemDto(this CartItem i)
        {
            var dto = new CartItemDto
            {
                Id = i.Id,
                Description = i.Description,
                ItemId = i.ItemId,
                Unit = i.Unit is not null ? new Carts.Contracts.UnitDto
                {
                    Id = 0,
                    Name = "piece",
                    Code = i.Unit,
                } : null,
                Price = i.Price,
                VatRate = i.VatRate,
                Quantity = i.Quantity,
                Charges = i.Charges.Select(CreateCartChargeDto),
                Charge = i.Charge(),
                Discounts = i.Discounts.Select(CreateCartDiscountDto),
                Discount = i.Discount(),
                Total = i.Total()
            };

            return dto;
        }

        public static CartChargeDto CreateCartChargeDto(this CartCharge arg)
        {
            var chargeDto = new CartChargeDto
            {
                Id = arg.Id,
                Amount = arg.Amount,
                Percent = arg.Percent,
                Description = arg.Description,
                Quantity = arg.Quantity,
                ChargeId = arg.ChargeId
            };

            if (arg.Cart is not null)
            {
                chargeDto.Total = arg.Total(arg.Cart);
            }
            else if (arg.CartItem is not null)
            {
                chargeDto.Total = arg.Total(arg.CartItem);
            }

            return chargeDto;
        }

        public static CartDiscountDto CreateCartDiscountDto(this CartDiscount arg)
        {
            var discountDto = new CartDiscountDto
            {
                Id = arg.Id,
                Amount = arg.Amount,
                Percent = arg.Percent,
                Description = arg.Description,
                Quantity = arg.Quantity,
                DiscountId = arg.DiscountId
            };

            if (arg.Cart is not null)
            {
                discountDto.Total = arg.Total(arg.Cart);
            }
            else if (arg.CartItem is not null)
            {
                discountDto.Total = arg.Total(arg.CartItem);
            }

            return discountDto;
        }
    }
}