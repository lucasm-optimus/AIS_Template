using Tilray.Integrations.Core.Domain.Aggregates.Invoices.Events;

namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices.Commands;

public class CreateInvoicesInObeerCommand : InvoiceGroup, ICommand<InvoicesProcessed>
{
}
