namespace Tilray.Integrations.Storage.Blob;

internal class BlobService(BlobServiceClient blobServiceClient, BlobSettings blobSettings, ILogger<BlobService> logger) : IBlobService
{
    #region Private members

    private readonly BlobServiceClient _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));

    #endregion

    #region Private Methods

    private async Task<BlobClient> GetBlobClientAsync(string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(blobSettings.BlobContainerName);
        await containerClient.CreateIfNotExistsAsync();
        return containerClient.GetBlobClient(blobName);
    }

    #endregion

    #region Public Methods

    public async Task<string> UploadBlobContentAsync<T>(T content, string blobName)
    {
        var blobClient = await GetBlobClientAsync(blobName);
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content.ToJsonString()));
        await blobClient.UploadAsync(memoryStream);
        return blobName;
    }

    public async Task<string> DownloadBlobContentAsync(string blobName)
    {
        var blobClient = await GetBlobClientAsync(blobName);
        var downloadResponse = await blobClient.DownloadContentAsync();
        logger.LogInformation("Successfully downloaded blob content for: {BlobName}", blobName);

        return downloadResponse.Value.Content.ToString();
    }

    public async Task DeleteBlobAsync(string blobName)
    {
        var blobClient = await GetBlobClientAsync(blobName);
        await blobClient.DeleteIfExistsAsync();
    }

    #endregion
}
