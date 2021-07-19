using System;

namespace Carts.Contracts
{
    public class CartItemAddedEvent
    {
        public CartItemAddedEvent()
        {
        }

        public CartItemAddedEvent(Guid cartId, Guid cartItemId)
        {
            CartId = cartId;
            CartItemId = cartItemId;
        }

        public Guid CartId { get; set; }

        public Guid CartItemId { get; set; }
    }

    public class CartClearedEvent
    {
        public CartClearedEvent(Guid cartId)
        {
            CartId = cartId;
        }

        public Guid CartId { get; }
    }

    public class CartItemRemovedEvent
    {
        public CartItemRemovedEvent(Guid cartId, Guid cartItemId)
        {
            CartId = cartId;
            CartItemId = cartItemId;
        }

        public Guid CartId { get; }

        public Guid CartItemId { get; }
    }

    public class CartItemQuantityUpdatedEvent
    {
        public CartItemQuantityUpdatedEvent(Guid cartId, Guid cartItemId, double oldQuantity, double newQuantity)
        {
            CartId = cartId;
            CartItemId = cartItemId;
            OldQuantity = oldQuantity;
            NewQuantity = newQuantity;
        }

        public Guid CartId { get; }

        public Guid CartItemId { get; set; }

        public double OldQuantity { get; set; }

        public double NewQuantity { get; set; }
    }
}