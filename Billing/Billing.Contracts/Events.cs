namespace Billing.Contracts;

public class ReceiptCreatedEvent
{
    public ReceiptCreatedEvent(int receiptNo)
    {
        ReceiptNo = receiptNo;
    }

    public int ReceiptNo { get; }
}