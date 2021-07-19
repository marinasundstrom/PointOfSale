namespace Billing.Contracts
{
    public class CreateReceiptCommand
    {
        internal CreateReceiptCommand()
        {
        }

        public CreateReceiptCommand(CreateReceiptDto dto)
        {
            Dto = dto;
        }

        public CreateReceiptDto Dto { get; internal set; } = null!;
    }

    internal class CreateReceiptCommandResponse
    {
        internal CreateReceiptCommandResponse()
        {
        }

        public CreateReceiptCommandResponse(int receiptNo)
        {
            ReceiptNo = receiptNo;
        }

        public int ReceiptNo { get; internal set; }
    }

    public class SendReceiptByEmailCommand
    {
        public SendReceiptByEmailCommand(int receiptNo)
        {
            ReceiptNo = receiptNo;
        }

        public int ReceiptNo { get; set; }
    }

    public class SendReceiptByEmailCommandResponse
    {

    }
}