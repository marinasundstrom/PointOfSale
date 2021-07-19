using System.Collections.Generic;

namespace Payments.Contracts
{
    public class MakePaymentRequestCommand
    {
        public string PaymentMethod { get; set; } = null!;

        public string Data { get; set; } = null!;

        public decimal AmountRequested { get; set; }

        public decimal? AmountConfirmed { get; set; }

        public DateTime Due { get; set; }

        public string OrderRef { get; set; } = null!;
    }

    public class MakePaymentRequestCommandResponse
    {

    }

    public class CancelPaymentCommand
    {
        public string OrderRef { get; set; } = null!;
    }

    public class CancelPaymentCommandResponse
    {

    }
}