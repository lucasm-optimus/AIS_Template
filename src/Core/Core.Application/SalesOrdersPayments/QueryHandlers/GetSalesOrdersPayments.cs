using Tilray.Integrations.Core.Domain.Aggregates.SalesOrdersPayments;

namespace Tilray.Integrations.Core.Application.SalesOrdersPayments.QueryHandlers;

public record GetSalesOrdersPayments : QueryManyBase<SalesOrderPayment>
{
}
