using System;

using OrderPriceCalculator;

using Payments.Contracts;
using Payments.Domain.Entities;

namespace Payments
{
    public static class Mappings
    {
        public static PaymentDto ToPaymentDto(this Payment payment)
        {
            return new PaymentDto()
            {
                Id = payment.Id,
                Status = (Payments.Contracts.PaymentStatus)payment.Status,
                PaymentMethod = payment.PaymentMethod,
                Data = payment.Data,
                AmountRequested = payment.AmountRequested,
                AmountConfirmed = payment.AmountConfirmed,
                Due = payment.Due,
                OrderRef = payment.OrderRef
            };
        }
    }
}