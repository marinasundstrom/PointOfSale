using System;

namespace Carts.Contracts
{
    public class AddCartItemCommand
    {
        public AddCartItemCommand(Guid cartId, string? description, string? itemId, string? unit, double quantity)
        {
            CartId = cartId;
            Description = description;
            ItemId = itemId;
            Unit = unit;
            Quantity = quantity;
        }

        public Guid CartId { get; }

        public string? Description { get; }

        public string? ItemId { get; }

        public string? Unit { get; }

        public double Quantity { get; }
    }

    public class AddCartItemCommandResponse
    {
        public AddCartItemCommandResponse()
        {
        }

        public AddCartItemCommandResponse(Guid cartId, Guid cartItemId)
        {
            CartId = cartId;
            CartItemId = cartItemId;
        }

        public Guid CartId { get; set; }

        public Guid CartItemId { get; set; }
    }

    public class ClearCartCommand
    {
        public ClearCartCommand(Guid cartId)
        {
            CartId = cartId;
        }

        public Guid CartId { get; }
    }

    public class ClearCartCommandResponse
    {
        public ClearCartCommandResponse()
        {
        }

        public ClearCartCommandResponse(Guid cartId)
        {
            CartId = cartId;
        }

        public Guid CartId { get; set; }
    }

    public class CreateCartCommand
    {

    }

    public class CreateCartCommandResponse
    {
        public CreateCartCommandResponse()
        {
        }

        public CreateCartCommandResponse(Guid cartId)
        {
            CartId = cartId;
        }

        public Guid CartId { get; set; }
    }

    public class RemoveCartItemCommand
    {
        public RemoveCartItemCommand(Guid cartId, Guid cartItemId)
        {
            CartId = cartId;
            CartItemId = cartItemId;
        }

        public Guid CartId { get; }

        public Guid CartItemId { get; }
    }

    public class RemoveCartItemCommandResponse
    {
        public RemoveCartItemCommandResponse()
        {
        }

        public RemoveCartItemCommandResponse(Guid cartId, Guid cartItemId)
        {
            CartId = cartId;
            CartItemId = cartItemId;
        }

        public Guid CartId { get; set; }

        public Guid CartItemId { get; set; }
    }

    public class UpdateCartItemQuantityCommand
    {
        public UpdateCartItemQuantityCommand(Guid cartId, Guid cartItemId, double quantity)
        {
            CartId = cartId;
            CartItemId = cartItemId;
            Quantity = quantity;
        }

        public Guid CartId { get; }

        public Guid CartItemId { get; }

        public double Quantity { get; }
    }

    public class UpdateCartItemQuantityCommandResponse
    {
        public UpdateCartItemQuantityCommandResponse()
        {
        }

        public UpdateCartItemQuantityCommandResponse(Guid cartId, Guid cartItemId)
        {
            CartId = cartId;
            CartItemId = cartItemId;
        }

        public Guid CartId { get; set; }

        public Guid CartItemId { get; set; }
    }
}