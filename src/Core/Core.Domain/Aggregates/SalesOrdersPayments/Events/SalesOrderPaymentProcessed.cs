using MediatR;

namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrdersPayments.Events;

public class SalesOrderPaymentProcessed(string salesOrderPaymentId, string transactionId) : IDomainEvent
{
    public string SalesOrderPaymentId { get; } = salesOrderPaymentId;
    public string TransactionId { get; } = transactionId;
}

public record CaptureAttemptedOnInvalidTransaction(string TransactionId, string TransactionStatus) : IDomainEvent;

public record CapturePaymentFailed(string TransactionId, string ErrorMessage) : IDomainEvent;
