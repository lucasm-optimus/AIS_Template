using Tilray.Integrations.Core.Domain.Aggregates.SalesOrdersPayments.Events;

namespace Tilray.Integrations.Core.Domain.Aggregates.SalesOrdersPayments.Commands;

public class CapturePaymentInAuthorizeNetCommand : SalesOrderPayment, ICommand<SalesOrderPaymentProcessed>
{
}
