using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace Checkout.Hubs
{

    public class CatalogHub : Hub<ICatalogClient>
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.CatalogItemUpdated(null!);
        }
    }
}