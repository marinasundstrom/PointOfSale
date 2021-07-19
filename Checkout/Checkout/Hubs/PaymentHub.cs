using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace Checkout.Hubs
{

    public class PaymentHub : Hub<IPaymentClient>
    {

    }
}