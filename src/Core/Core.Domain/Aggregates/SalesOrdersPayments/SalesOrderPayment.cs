namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrdersPayments;

public class SalesOrderPayment
{
    public string Id { get; set; }
    public string Division { get; set; }
    public string PaymentId { get; set; }
    public string TransactionId { get; set; }
    public string PaymentGateway { get; set; }
    public decimal PaymentAmount { get; set; }

    public static SalesOrderPayment Create(string division, string paymentId, string transactionId, string paymentGateway,
        decimal paymentAmount)
    {
        return new SalesOrderPayment
        {
            Division = division,
            PaymentId = paymentId,
            TransactionId = transactionId,
            PaymentGateway = paymentGateway,
            PaymentAmount = paymentAmount
        };
    }
}

public class SalesOrderPaymentAlreadyProcessedError : Error
{
    public string TransactionId { get; }
    public string Status { get; }

    public SalesOrderPaymentAlreadyProcessedError(string transactionId, string status)
        : base($"Transaction {transactionId} is already {status}.")
    {
        TransactionId = transactionId;
        Status = status;
    }
}
