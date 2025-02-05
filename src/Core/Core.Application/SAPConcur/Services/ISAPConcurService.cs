namespace Tilray.Integrations.Core.Application.SAPConcur.Services
{
    public interface ISAPConcurService
    {
        Task<Result<IEnumerable<EcomSalesOrder>>> GetSalesOrders();
    }
}
