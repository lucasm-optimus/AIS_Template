namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface ISharepointService
{
    Task<Result> UploadFileAsync(IEnumerable<Invoice> invoices, CompanyReference companyReference);
    Task<Result> UploadFileAsync<T>(IEnumerable<T> content, CompanyReference companyReference);
}
