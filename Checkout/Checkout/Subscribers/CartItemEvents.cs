using System;
using System.Linq;
using System.Threading.Tasks;

using Carts.Contracts;

using Catalog.Client;

using Checkout.Hubs;

using Marketing.Contracts;

using MassTransit;

using Microsoft.AspNetCore.SignalR;


namespace Checkout.Subscribers
{
    public class CartItemdEventHandler : IConsumer<CartItemAddedEvent>, IConsumer<CartItemRemovedEvent>, IConsumer<CartItemQuantityUpdatedEvent>
    {
        private readonly IHubContext<CartHub, ICartClient> cartHubContext;
        private readonly IItemsClient catalogItemsClient;
        private readonly IRequestClient<GetDiscountsQuery> getDiscountsQueryClient;
        private readonly Carts.Client.ICartClient cartClient;

        public CartItemdEventHandler(
            IItemsClient catalogItemsClient,
            IRequestClient<GetDiscountsQuery> getDiscountsQueryClient,
            Carts.Client.ICartClient cartClient,
            IHubContext<CartHub, ICartClient> cartHubContext)
        {
            this.catalogItemsClient = catalogItemsClient;
            this.getDiscountsQueryClient = getDiscountsQueryClient;
            this.cartHubContext = cartHubContext;
            this.cartClient = cartClient;
        }

        public async Task Consume(ConsumeContext<CartItemAddedEvent> consumeContext)
        {
            var message = consumeContext.Message;

            await CheckForDiscountsAndApply(message.CartId, message.CartItemId);

            await cartHubContext.Clients.All.ItemAdded(message);
        }

        public async Task Consume(ConsumeContext<CartItemRemovedEvent> consumeContext)
        {
            var message = consumeContext.Message;

            var cartItems = await cartClient.GetItemsAsync(message.CartId);

            foreach (var cartItem in cartItems)
            {
                await CheckForDiscountsAndApply(message.CartId, cartItem.Id);
            }

            await cartHubContext.Clients.All.ItemRemoved(message);
        }

        public async Task Consume(ConsumeContext<CartItemQuantityUpdatedEvent> consumeContext)
        {
            var message = consumeContext.Message;

            await CheckForDiscountsAndApply(message.CartId, message.CartItemId);

            await cartHubContext.Clients.All.ItemQuantityUpdated(message);
        }

        async Task CheckForDiscountsAndApply(Guid cartId, Guid cartItemId)
        {
            var item = await cartClient.GetItemAsync(cartId, cartItemId);

            var catalogItem = await catalogItemsClient.GetItemByItemIdAsync(item.ItemId);

            if (catalogItem is not null)
            {
                var result = await getDiscountsQueryClient.GetResponse<GetDiscountsQueryResponse>(new GetDiscountsQuery());
                var discounts = result.Message.Discounts;

                var discount = discounts.FirstOrDefault(x => x.ItemId == catalogItem.Id);
                var discountAsOtherProduct = discounts.FirstOrDefault(x => x.OtherItemId == catalogItem.Id);

                if (discount is not null)
                {
                    await DiscountProduct(cartId, item, catalogItem, discount);
                }

                if (discountAsOtherProduct is not null)
                {
                    await DiscountOtherProduct(cartId, item, catalogItem, discountAsOtherProduct);
                }
            }
        }

        async Task DiscountProduct(Guid cartId, Carts.Client.CartItemDto cartItem, Catalog.Client.CatalogItemDto catalogItem, Marketing.Contracts.DiscountDto discount)
        {
            var itemDiscount = cartItem.Discounts.FirstOrDefault(d => d.DiscountId == discount.Id);

            if (discount.OtherItemId is null)
            {
                // The discount is not dependant on another catalog item.

                if (itemDiscount is not null)
                {
                    if (cartItem.Quantity < discount.Quantity)
                    {
                        await cartClient.RemoveDiscountFromItemAsync(cartId, cartItem.Id, itemDiscount.Id);
                    }
                }
                else
                {
                    if (cartItem.Quantity == discount.Quantity)
                    {
                        await cartClient.AddDiscountToItemAsync(cartId, cartItem.Id, new Carts.Client.AddDiscountDto()
                        {
                            DiscountId = discount.Id,
                            Quantity = discount.Quantity,
                            Description = discount.Name,
                            Amount = -discount.Amount,
                            Percent = -discount.Percent
                        });
                    }
                }
            }
            else
            {
                // The discount is dependant on another catalog item.

                var items = await cartClient.GetItemsAsync(cartId);

                var catalogItem2 = await catalogItemsClient.GetItemByItemIdAsync(discount.OtherItemId!);

                var otherItem = items.FirstOrDefault(d => d.ItemId.ToString() == catalogItem2.Id);

                if (otherItem is not null)
                {
                    if (cartItem.Discounts.Any(d => d.DiscountId == discount.Id))
                        return;

                    await cartClient.AddDiscountToItemAsync(cartId, cartItem.Id, new Carts.Client.AddDiscountDto()
                    {
                        DiscountId = discount.Id,
                        Quantity = discount.Quantity,
                        Description = discount.Name,
                        Amount = -discount.Amount,
                        Percent = -discount.Percent
                    });
                }
                else
                {
                    if (itemDiscount is not null)
                    {
                        await cartClient.RemoveDiscountFromItemAsync(cartId, cartItem.Id, itemDiscount.Id);
                    }
                }
            }
        }

        async Task DiscountOtherProduct(Guid cartId, Carts.Client.CartItemDto cartItem, Catalog.Client.CatalogItemDto catalogItem, Marketing.Contracts.DiscountDto discount)
        {
            var discountedCatalogItem = await catalogItemsClient.GetItemByItemIdAsync(discount.ItemId);

            var cartItems = await cartClient.GetItemsAsync(cartId);

            var otherItem = cartItems.FirstOrDefault(d => d.ItemId.ToString() == discountedCatalogItem.Id); ;

            if (otherItem is not null)
            {
                if (otherItem.Discounts.Any(d => d.DiscountId == discount.Id))
                    return;

                await cartClient.AddDiscountToItemAsync(cartId, otherItem.Id, new Carts.Client.AddDiscountDto()
                {
                    DiscountId = discount.Id,
                    Description = discount.Name,
                    Amount = -discount.Amount,
                    Percent = -discount.Percent
                });
            }
            else { }
        }
    }
}