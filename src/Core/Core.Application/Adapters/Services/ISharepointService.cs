namespace Tilray.Integrations.Core.Application.Adapters.Services;

public interface ISharepointService
{
    Task<Result> UploadFileAsync<T>(List<T> content, CompanyReference companyReference);
}
