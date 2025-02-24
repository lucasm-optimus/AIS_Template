namespace Tilray.Integrations.Core.Domain.Aggregates.Invoices.Commands;

public record ImportInvoicesInRootstockCommand(string InvoiceGroupBlobName) : ICommand<InvoicesAggCreated> { }
