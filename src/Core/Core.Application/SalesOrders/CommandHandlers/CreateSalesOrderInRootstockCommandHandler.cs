namespace Tilray.Integrations.Core.Application.SalesOrders.CommandHandlers;

public class CreateSalesOrderInRootstockCommandHandler(IRootstockService rootstockService) : ICommandHandler<CreateSalesOrderInRootstockCommand>
{
    public async Task<Result> Handle(CreateSalesOrderInRootstockCommand request, CancellationToken cancellationToken)
    {
        return await rootstockService.CreateSalesOrderAsync(request.SalesOrder);
    }
}
