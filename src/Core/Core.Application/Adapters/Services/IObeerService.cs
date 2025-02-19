namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface IObeerService
{
    Task<Result<(List<GrpoLineItemError> ErrorsGrpo, List<NonPOLineItemError> ErrorsNonPO)>> CreateInvoiceAsync(Invoice invoice);
}
