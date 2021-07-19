using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

using Payments.Contracts;
using Payments.Domain.Entities;
using Payments.Domain.Enums;
using Payments.Infrastructure.Persistence;

namespace Payments.Application
{
    public class MakePaymentRequestCommandHandler : IConsumer<MakePaymentRequestCommand>
    {
        private readonly ILogger<MakePaymentRequestCommandHandler> _logger;
        private readonly PaymentsContext context;

        public MakePaymentRequestCommandHandler(
            ILogger<MakePaymentRequestCommandHandler> logger,
            PaymentsContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Consume(ConsumeContext<MakePaymentRequestCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var payment = new Payment()
            {
                Id = Guid.NewGuid(),
                Status = Domain.Enums.PaymentStatus.Pending,
                PaymentMethod = message.PaymentMethod,
                Data = message.Data,
                AmountRequested = message.AmountRequested,
                Due = message.Due,
                OrderRef = message.OrderRef,
            };

            context.Payments.Add(payment);

            await context.SaveChangesAsync();

            var response = new MakePaymentRequestCommandResponse();

            await consumeContext.RespondAsync<MakePaymentRequestCommandResponse>(response);

            await SimulateConfirmation(context, payment, message.OrderRef);
        }

        async Task SimulateConfirmation(PaymentsContext context, Payment payment, string orderRef)
        {
            //INFO: Simulate PaymentConfirmed

            payment.Status = Domain.Enums.PaymentStatus.Confirmed;
            payment.AmountConfirmed = payment.AmountRequested;

            payment.DomainEvents.Add(new PaymentConfirmedEvent() { OrderRef = orderRef });

            await Task.Delay(5000);

            await context.SaveChangesAsync();
        }
    }

    public class CancelPaymentCommandHandler : IConsumer<CancelPaymentCommand>
    {
        private readonly ILogger<CancelPaymentCommandHandler> _logger;
        private readonly PaymentsContext context;

        public CancelPaymentCommandHandler(
            ILogger<CancelPaymentCommandHandler> logger,
            PaymentsContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Consume(ConsumeContext<CancelPaymentCommand> consumeContext)
        {
            var message = consumeContext.Message;

            var payments = await context.Payments
                .Where(p => p.Status != Domain.Enums.PaymentStatus.Cancelled)
                .Where(p => p.OrderRef == message.OrderRef)
                .ToArrayAsync();

            if (!payments.Any())
            {
                throw new Exception();
            }

            var response = new CancelPaymentCommandResponse();

            await consumeContext.RespondAsync<CancelPaymentCommandResponse>(response);

            foreach (var payment in payments)
            {
                payment.Status = Domain.Enums.PaymentStatus.Cancelled;

                payment.DomainEvents.Add(new PaymentCancelledEvent() { OrderRef = message.OrderRef });
            }

            await context.SaveChangesAsync();
        }
    }
}