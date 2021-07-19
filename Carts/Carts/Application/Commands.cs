using System;
using System.Linq;
using System.Threading.Tasks;

using Carts.Contracts;
using Carts.Domain.Entities;
using Carts.Infrastructure.Persistence;

using Catalog.Client;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OrderPriceCalculator;

namespace Carts.Application
{
    public class CreateCartCommandHandler : IConsumer<CreateCartCommand>
    {
        private readonly ILogger<CreateCartCommandHandler> _logger;
        private readonly CartContext context;
        private readonly IItemsClient catalogItemsClient;
        private readonly IBus bus;

        public CreateCartCommandHandler(
            ILogger<CreateCartCommandHandler> logger,
            CartContext context,
            IItemsClient catalogItemsClient,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.catalogItemsClient = catalogItemsClient;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<CreateCartCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var cart = new Cart();

            context.Carts.Add(cart);

            await context.SaveChangesAsync();

            await consumeContext.RespondAsync<CreateCartCommandResponse>(new CreateCartCommandResponse());
        }
    }

    public class ClearCartCommandHandler : IConsumer<ClearCartCommand>
    {
        private readonly ILogger<ClearCartCommandHandler> _logger;
        private readonly CartContext context;
        private readonly IItemsClient catalogItemsClient;
        private readonly IBus bus;

        public ClearCartCommandHandler(ILogger<ClearCartCommandHandler> logger, CartContext context, IItemsClient catalogItemsClient, IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.catalogItemsClient = catalogItemsClient;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<ClearCartCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var cart = await context.Carts
                .Include(c => c.Items)
                .ThenInclude(c => c.Charges)
                .Include(c => c.Items)
                .ThenInclude(c => c.Discounts)
                .Include(c => c.Charges)
                .Include(c => c.Discounts)
                .FirstOrDefaultAsync(c => c.Id == message.CartId);

            if (cart is null)
            {
                throw new Exception();
            }

            foreach (var cartItem in cart.Items)
            {
                cartItem.Discounts.Clear();
            }

            cart.Items.Clear();
            cart.Charges.Clear();
            cart.Discounts.Clear();

            await context.SaveChangesAsync();

            await bus.Publish(new CartClearedEvent(cart.Id));

            await consumeContext.RespondAsync<ClearCartCommandResponse>(new ClearCartCommandResponse(cart.Id));
        }
    }

    public class AddCartItemCommandHandler : IConsumer<AddCartItemCommand>
    {
        private readonly ILogger<AddCartItemCommandHandler> _logger;
        private readonly CartContext context;
        private readonly IItemsClient catalogItemsClient;
        private readonly IBus bus;

        public AddCartItemCommandHandler(
            ILogger<AddCartItemCommandHandler> logger,
            CartContext context,
            IItemsClient catalogItemsClient,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.catalogItemsClient = catalogItemsClient;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<AddCartItemCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var cart = await context.Carts
                .Where(c => c.Id == message.CartId)
                .Include(c => c.Items)
                .FirstOrDefaultAsync();

            if (cart is null)
            {
                cart = new Cart();
                context.Carts.Add(cart);
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.ItemId == message.ItemId);

            if (cartItem is null)
            {
                CatalogItemDto? catalogItem = null;

                if (message.ItemId is not null)
                {
                    catalogItem = await catalogItemsClient.GetItemByItemIdAsync(message.ItemId);
                }

                cartItem = new CartItem()
                {
                    Id = Guid.NewGuid(),
                    Cart = cart,
                    Description = message.Description ?? catalogItem!.Name,
                    ItemId = message.ItemId!,
                    Unit = message.Unit ?? catalogItem!.Unit.Code,
                    Quantity = message.Quantity,
                    Price = catalogItem!.VatIncluded ? catalogItem.Price : catalogItem.Price.AddVat(catalogItem.VatRate),
                    VatRate = catalogItem.VatRate
                };

                foreach (var catalogItemCharge in catalogItem.Charges)
                {
                    cartItem.Charges.Add(new CartCharge()
                    {
                        Id = Guid.NewGuid(),
                        //Quantity = catalogItemCharge.Quantity,
                        //Limit = catalogItemCharge.Limit,
                        Amount = catalogItemCharge.Amount,
                        Percent = catalogItemCharge.Percent,
                        //Total = catalogItemCharge.Total,
                        //Name = catalogItemCharge.Name,
                        Description = catalogItemCharge.Description ?? catalogItemCharge.Name,
                        //ChargeId = catalogItemCharge.ChargeId,
                    });
                }

                context.CartItems.Add(cartItem);

                await context.SaveChangesAsync();

                await bus.Publish(new CartItemAddedEvent(cart.Id, cartItem.Id));
            }
            else
            {

                var oldQuantity = cartItem.Quantity;

                cartItem.UpdateQuantity(cartItem.Quantity + message.Quantity);

                await context.SaveChangesAsync();

                await bus.Publish(new CartItemQuantityUpdatedEvent(cart.Id, cartItem.Id, oldQuantity, cartItem.Quantity));
            }

            await consumeContext.RespondAsync<AddCartItemCommandResponse>(new AddCartItemCommandResponse(cart.Id, cartItem.Id));
        }
    }

    public class RemoveCartItemCommandHandler : IConsumer<RemoveCartItemCommand>
    {
        private readonly ILogger<RemoveCartItemCommandHandler> _logger;
        private readonly CartContext context;
        private readonly IItemsClient catalogItemsClient;
        private readonly IBus bus;

        public RemoveCartItemCommandHandler(
            ILogger<RemoveCartItemCommandHandler> logger,
            CartContext context,
            IItemsClient catalogItemsClient,
            IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.catalogItemsClient = catalogItemsClient;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<RemoveCartItemCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var cart = context.Carts
                .Where(c => c.Id == message.CartId)
                .Include(c => c.Items)
                .ThenInclude(c => c.Charges)
                .Include(c => c.Items)
                .ThenInclude(c => c.Discounts)
                .FirstOrDefault();

            if (cart is null)
            {
                throw new Exception();
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.Id == message.CartItemId);

            if (cartItem is null)
            {
                throw new Exception();
            }

            cartItem.Charges.Clear();
            cartItem.Discounts.Clear();

            cart.Items.Remove(cartItem);

            await context.SaveChangesAsync();

            await bus.Publish(new CartItemRemovedEvent(cart.Id, cartItem.Id));

            await consumeContext.RespondAsync<RemoveCartItemCommandResponse>(new RemoveCartItemCommandResponse(cart.Id, cartItem.Id));
        }
    }

    public class UpdateCartItemQuantityCommandHandler : IConsumer<UpdateCartItemQuantityCommand>
    {
        private readonly ILogger<UpdateCartItemQuantityCommandHandler> _logger;
        private readonly CartContext context;
        private readonly IItemsClient catalogItemsClient;
        private readonly IBus bus;

        public UpdateCartItemQuantityCommandHandler(ILogger<UpdateCartItemQuantityCommandHandler> logger, CartContext context, IItemsClient catalogItemsClient, IBus bus)
        {
            _logger = logger;
            this.context = context;
            this.catalogItemsClient = catalogItemsClient;
            this.bus = bus;
        }

        public async Task Consume(ConsumeContext<UpdateCartItemQuantityCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var cart = context.Carts
                .Where(c => c.Id == message.CartId)
                .Include(c => c.Items)
                .FirstOrDefault();

            if (cart is null)
            {
                throw new Exception();
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.Id == message.CartItemId);

            if (cartItem is null)
            {
                throw new Exception();
            }

            var oldQuantity = cartItem.Quantity;

            cartItem.UpdateQuantity(message.Quantity);

            await context.SaveChangesAsync();

            await bus.Publish(new CartItemQuantityUpdatedEvent(cart.Id, cartItem.Id, oldQuantity, cartItem.Quantity));

            await consumeContext.RespondAsync<UpdateCartItemQuantityCommandResponse>(new UpdateCartItemQuantityCommandResponse(cart.Id, cartItem.Id));
        }
    }
}