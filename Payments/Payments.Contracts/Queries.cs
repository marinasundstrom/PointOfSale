using System.Collections.Generic;

namespace Payments.Contracts
{
    public class GetPaymentsPaymentsQuery
    {
    }

    public class GetPaymentsPaymentsQueryResponse
    {
        public IEnumerable<PaymentDto> Payments { get; set; } = null!;
    }

    public class GetPaymentsByOrderRefQuery
    {
        public GetPaymentsByOrderRefQuery(string orderRef)
        {
            OrderRef = orderRef;
        }

        public string OrderRef { get; set; }
    }

    public class GetPaymentsByOrderRefQueryResponse
    {
        public IEnumerable<PaymentDto> Payments { get; set; } = null!;
    }
}