using System.Threading.Tasks;

using Payments.Contracts;

namespace Checkout.Hubs
{
    public interface IPaymentClient
    {
        Task PaymentConfirmed(PaymentConfirmedEvent ev);

        Task PaymentCancelled(PaymentCancelledEvent ev);
    }
}