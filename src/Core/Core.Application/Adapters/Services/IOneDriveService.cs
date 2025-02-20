
namespace Tilray.Integrations.Core.Application.Adapters.Service
{
    public interface IOneDriveService
    {
        Task<Result<bool>> PutFileAsync(string filePathAndName, string fileContent, string contentType);
    }
}
