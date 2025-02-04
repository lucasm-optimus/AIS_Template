using Microsoft.AspNetCore.Mvc;
using Tilray.Integrations.Core.Domain.Aggregates.SalesOrders;

namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface IRootstockService
{
    Task<Result<IEnumerable<CompanyReference>>> GetAllCompanyReferencesAsync();
    Task<Result<IEnumerable<SalesOrder>>> ValidateSalesOrders(IEnumerable<SalesOrder> salesOrders);
    Task<Result> CreateSalesOrderAsync(SalesOrder salesOrder);
}
