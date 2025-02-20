using FluentResults;
using System.Text;

namespace Tilray.Integrations.Services.OneDrive
{
    /// <summary>
    /// Service for uploading files to OneDrive.
    /// </summary>
    public class OneDriveService : IOneDriveService
    {
        private readonly HttpClient client;
        private readonly OneDriveSettings oneDriveSettings;
        private readonly ILogger<OneDriveService> logger;


        public OneDriveService(HttpClient client, OneDriveSettings oneDriveSettings, ILogger<OneDriveService> logger)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.oneDriveSettings = oneDriveSettings ?? throw new ArgumentNullException(nameof(oneDriveSettings));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private string BuildUploadUri(string filePathAndName)
        {
            filePathAndName = Uri.EscapeDataString(filePathAndName);
            return $"/v1.0/users/{oneDriveSettings.Username}/drive/root:/{filePathAndName}:/content";
        }


        public async Task<Result<bool>> PutFileAsync(string filePathAndName, string fileContent, string contentType)
        {
            var requestUri = BuildUploadUri(filePathAndName);


            var fileBytes = Encoding.UTF8.GetBytes(fileContent); // Convert file content to bytes

            // 🔹 Validate file size using `oneDriveSettings.FileSizeLimit`
            if (fileBytes.Length >= int.Parse(oneDriveSettings.FileSizeLimit))
            {
                var errorMessage = $"File size limit exceeded. Max allowed: {oneDriveSettings.FileSizeLimit} bytes";
                logger.LogError(errorMessage);
                return Result.Fail<bool>(errorMessage);
            }




            using var content = new ByteArrayContent(fileBytes);

            content.Headers.ContentType = new MediaTypeHeaderValue(contentType); // Set content type header


            var response = await client.PutAsync(requestUri, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("File upload failed. Status: {StatusCode}, Error: {Error}", response.StatusCode, responseBody);
                return Result.Fail<bool>($"File upload failed. Error: {responseBody}");
            }
            logger.LogInformation("File uploaded successfully to OneDrive: {FilePath}", filePathAndName);
            return Result.Ok(true);
        }
    }
}
