using System;
using System.Linq;
using System.Threading.Tasks;

using Checkout.Application.Services;
using Checkout.Contracts;
using Checkout.Hubs;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

using Payments.Contracts;

using Sales.Client;

namespace Checkout.Subscribers
{
    public class PaymentEventsHandler : IConsumer<PaymentConfirmedEvent>, IConsumer<PaymentCancelledEvent>
    {
        private readonly IOrdersClient ordersClient;
        private readonly CheckoutFinalizationService checkoutFinalizationService;
        private readonly IHubContext<PaymentHub, IPaymentClient> paymentHubContext;

        public PaymentEventsHandler(
            IOrdersClient ordersClient,
            CheckoutFinalizationService checkoutFinalizationService,
            IHubContext<PaymentHub, IPaymentClient> paymentHubContext)
        {
            this.ordersClient = ordersClient;
            this.checkoutFinalizationService = checkoutFinalizationService;
            this.paymentHubContext = paymentHubContext;
        }

        public async Task Consume(ConsumeContext<PaymentConfirmedEvent> consumeContext)
        {
            var message = consumeContext.Message;

            var orderId = int.Parse(message.OrderRef);

            await ordersClient.UpdateOrderStatusAsync(orderId, "placed");

            await checkoutFinalizationService.Finalize(orderId);

            await paymentHubContext.Clients.All.PaymentConfirmed(message);
        }

        public async Task Consume(ConsumeContext<PaymentCancelledEvent> consumeContext)
        {
            var message = consumeContext.Message;

            var orderId = int.Parse(message.OrderRef);

            await ordersClient.UpdateOrderStatusAsync(orderId, "cancelled");

            await paymentHubContext.Clients.All.PaymentCancelled(message);
        }
    }
}