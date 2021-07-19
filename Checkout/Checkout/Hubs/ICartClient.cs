using System.Threading.Tasks;

using Carts.Contracts;

namespace Checkout.Hubs
{
    public interface ICartClient
    {
        Task ReceiveMessage(string user, string message);

        Task ItemAdded(CartItemAddedEvent ev);

        Task ItemRemoved(CartItemRemovedEvent ev);

        Task ItemQuantityUpdated(CartItemQuantityUpdatedEvent ev);
    }
}