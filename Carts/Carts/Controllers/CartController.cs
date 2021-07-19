using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Carts.Contracts;
using Carts.Domain.Entities;
using Carts.Infrastructure.Persistence;

using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OrderPriceCalculator;

namespace Carts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Produces("application/hal+json")]
    //[Produces("application/prs.hal-forms+json")]
    public class CartController : ControllerBase
    {
        private readonly ILogger<CartController> _logger;
        private readonly CartContext context;

        public CartController(
            ILogger<CartController> logger,
            CartContext context)
        {
            _logger = logger;
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<CartDto>> GetCarts([FromServices] IRequestClient<GetCartsQuery> client)
        {
            var response = await client.GetResponse<GetCartsQueryResponse>(
                new GetCartsQuery()
            );

            return response.Message.Carts;
        }

        [HttpGet("My")]
        public async Task<CartDto> GetCart([FromServices] IRequestClient<GetCartQuery> client)
        {
            var response = await client.GetResponse<CartDto>(
                new GetCartQuery()
            );

            return response.Message;
        }

        [HttpGet("{id}")]
        public async Task<CartDto> GetCartById([FromServices] IRequestClient<GetCartByIdQuery> client, Guid id)
        {
            var response = await client.GetResponse<CartDto>(
                new GetCartByIdQuery()
                {
                    Id = id
                }
            );

            return response.Message;
        }

        [HttpGet("GetCartByTag")]
        public async Task<CartDto> GetCartByTag([FromServices] IRequestClient<GetCartByTagQuery> client, string? tag = null)
        {
            var response = await client.GetResponse<CartDto>(
                new GetCartByTagQuery()
                {
                    Tag = tag
                }
            );

            return response.Message;
        }

        [HttpGet("{cartId}/Totals")]
        public async Task<CartTotalsDto> GetCartTotals(Guid cartId)
        {
            var cart = await context.Carts
                .Include(c => c.Items)
                .ThenInclude(c => c.Discounts)
                .Include(c => c.Discounts)
                .Where(c => c.Id == cartId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (cart is null)
            {
                cart = new Cart();
            }

            return new CartTotalsDto()
            {
                Totals = cart.Vat(),
                SubTotal = cart.Items.Sum(i => i.SubTotal()),
                Vat = cart.Items.Sum(i => i.Vat() * (decimal)i.Quantity),
                Discounts = cart.Discounts.Select(Mappers.CreateCartDiscountDto),
                Rounding = cart.Rounding(),
                Total = cart.Total(true)
            };
        }

        [HttpPost]
        public async Task<CartDto> CreateCart(string? tag = null)
        {
            //await bus.Publish(new CreateCartCommand());

            var cart = new Cart();
            cart.Id = Guid.NewGuid();

            if (tag is not null && await context.Carts.AnyAsync((x => x.Tag == tag)))
            {
                throw new Exception("Cart with tag already exists.");
            }

            cart.Tag = tag;

            context.Carts.Add(cart);

            await context.SaveChangesAsync();

            //await bus.Publish(new CartItemAddedEvent(item.Id));

            return Mappers.CreateCartDto(cart);
        }

        [HttpPost("{cartId}/Items")]
        public async Task AddItem([FromServices] IRequestClient<AddCartItemCommand> client,
            Guid cartId, string? description, string? itemId, string? unit, double quantity = 1)
        {
            var r = await client.GetResponse<AddCartItemCommandResponse>(new AddCartItemCommand(cartId, description, itemId, unit, quantity));
        }

        [HttpGet("{cartId}/Items")]
        public async Task<IEnumerable<CartItemDto>> GetItems(Guid cartId)
        {
            var cart = await context.Carts
                .Include(c => c.Items)
                .ThenInclude(c => c.Discounts)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == cartId);

            if (cart is null)
            {
                cart = new Cart();
            }

            return cart.Items.Select(Mappers.CreateCartItemDto);
        }

        [HttpGet("{cartId}/Items/{cartItemId}")]
        public async Task<CartItemDto> GetItem(Guid cartId, Guid cartItemId)
        {
            var cart = await context.Carts
                .Include(c => c.Items)
                .ThenInclude(c => c.Discounts)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart is null)
            {
                cart = new Cart();
            }

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

            if (item is null)
            {
                throw new Exception();
            }

            return Mappers.CreateCartItemDto(item);
        }

        [HttpDelete("{cartId}/Items/{cartItemId}")]
        public async Task RemoveItem([FromServices] IRequestClient<RemoveCartItemCommand> client, Guid cartId, Guid cartItemId)
        {
            await client.GetResponse<RemoveCartItemCommandResponse>(new RemoveCartItemCommand(cartId, cartItemId));
        }

        [HttpPut("{cartId}/Items/{cartItemId}/Quantity")]
        public async Task UpdateItemQuantity([FromServices] IRequestClient<UpdateCartItemQuantityCommand> client, Guid cartId, Guid cartItemId, [FromBody] double quantity)
        {
            await client.GetResponse<UpdateCartItemQuantityCommandResponse>(new UpdateCartItemQuantityCommand(cartId, cartItemId, quantity));
        }

        [HttpPost("{cartId}/Items/Clear")]
        public async Task ClearCart([FromServices] IRequestClient<ClearCartCommand> client, Guid cartId)
        {
            await client.GetResponse<ClearCartCommandResponse>(new ClearCartCommand(cartId));
        }

        [HttpPost("{cartId}/Discounts")]
        public async Task AddDiscount(Guid cartId, AddDiscountDto dto)
        {
            var cart = await context.Carts
                            .Include(c => c.Items)
                            .ThenInclude(c => c.Discounts)
                            .Include(c => c.Discounts)
                            .Where(c => c.Id == cartId)
                            .FirstOrDefaultAsync();

            if (cart is null)
            {
                throw new Exception();
            }

            if (dto.Percent is not null)
            {
                if (cart.Discounts.Any(x => x.Percent is null))
                {
                    throw new Exception("Cannot combine different discount types.");
                }

                if (cart.Discounts.Any(x => x.Percent is not null))
                {
                    throw new Exception("Cannot add another discount based on percentage.");
                }
            }

            if (dto.Percent is null && cart.Discounts.Any(x => x.Percent is not null))
            {
                throw new Exception("Cannot combine different discount types.");
            }

            context.CartDiscounts.Add(new CartDiscount
            {
                Id = Guid.NewGuid(),
                Cart = cart,
                Amount = dto.Amount * -1,
                Percent = dto.Percent * -1,
                Quantity = dto.Quantity,
                Description = dto.Description ?? string.Empty,
                DiscountId = dto.DiscountId
            });

            await context.SaveChangesAsync();
        }

        [HttpDelete("{cartId}/Discounts/{discountId}")]
        public async Task RemoveDiscount(Guid cartId, Guid discountId)
        {
            var cart = await context.Carts
                .Include(c => c.Items)
                .ThenInclude(c => c.Discounts)
                .Include(c => c.Discounts)
                .Where(c => c.Id == cartId)
                .FirstOrDefaultAsync();

            if (cart is null)
            {
                throw new Exception();
            }

            var discount = cart.Discounts.FirstOrDefault(x => x.Id == discountId);

            if (discount is null)
            {
                throw new Exception();
            }

            context.CartDiscounts.Remove(discount);

            await context.SaveChangesAsync();
        }

        [HttpPost("{cartId}/Items/{cartItemId}/Discounts")]
        public async Task AddDiscountToItem(Guid cartId, Guid cartItemId, AddDiscountDto dto)
        {
            var cart = await context.Carts
                            .Include(c => c.Items)
                            .ThenInclude(c => c.Discounts)
                            .Include(c => c.Discounts)
                            .Where(c => c.Id == cartId)
                            .FirstOrDefaultAsync();

            if (cart is null)
            {
                throw new Exception();
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

            if (cartItem is null)
            {
                throw new Exception();
            }

            if (dto.Percent is not null)
            {
                if (cartItem.Discounts.Any(x => x.Percent is null))
                {
                    throw new Exception("Cannot combine different discount types.");
                }

                if (cartItem.Discounts.Any(x => x.Percent is not null))
                {
                    throw new Exception("Cannot add another discount based on percentage.");
                }
            }

            if (dto.Percent is null && cartItem.Discounts.Any(x => x.Percent is not null))
            {
                throw new Exception("Cannot combine different discount types.");
            }

            context.CartDiscounts.Add(new CartDiscount
            {
                Id = Guid.NewGuid(),
                CartItem = cartItem,
                Amount = dto.Amount * -1,
                Percent = dto.Percent * -1,
                Quantity = dto.Quantity,
                Description = dto.Description ?? string.Empty,
                DiscountId = dto.DiscountId
            });

            await context.SaveChangesAsync();
        }

        [HttpDelete("{cartId}/Items/{cartItemId}/Discounts/{discountId}")]
        public async Task RemoveDiscountFromItem(Guid cartId, Guid cartItemId, Guid discountId)
        {
            var cart = await context.Carts
                .Include(c => c.Items)
                .ThenInclude(c => c.Discounts)
                .Include(c => c.Discounts)
                .Where(c => c.Id == cartId)
                .FirstOrDefaultAsync();

            if (cart is null)
            {
                throw new Exception();
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

            if (cartItem is null)
            {
                throw new Exception();
            }

            var discount = cartItem.Discounts.FirstOrDefault(x => x.Id == discountId);

            if (discount is null)
            {
                throw new Exception();
            }

            context.CartDiscounts.Remove(discount);

            await context.SaveChangesAsync();
        }
    }
}