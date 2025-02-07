namespace Tilray.Integrations.Core.Application.Adapters.Storage;

public interface IBlobService
{
    Task<string> UploadBlobContentAsync<T>(T content, string blobName);
    Task<string> DownloadBlobContentAsync(string blobName);
    Task DeleteBlobAsync(string blobName);
}
