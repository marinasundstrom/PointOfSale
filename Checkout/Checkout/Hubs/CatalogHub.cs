using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace Checkout.Hubs
{

    public class CartHub : Hub<ICartClient>
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);
        }
    }
}