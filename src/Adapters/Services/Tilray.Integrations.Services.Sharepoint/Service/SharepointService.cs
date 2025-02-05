using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System.Text;
using Tilray.Integrations.Core.Application.Adapters.Services;
using Tilray.Integrations.Services.Sharepoint.Startup;

namespace Tilray.Integrations.Services.Sharepoint.Service
{
    public class SharepointService(GraphServiceClient graphServiceClient, SharepointSettings sharepointSettings, ILogger<SharepointService> logger) : ISharepointService
    {
        private async Task<string> GetSiteIdAsync()
        {
            var site = await graphServiceClient.Sites["optimusinfo.sharepoint.com:/sites/Optimus-TechnicalUpgrade"]
                .GetAsync();

            return site.Id;
        }

        private async Task<string> GetDriveIdAsync()
        {
            var siteId = await GetSiteIdAsync();
            var drives = await graphServiceClient.Sites[siteId]
                .Drives
                .GetAsync();

            var drive = drives.Value.FirstOrDefault(d => d.Name.Equals("Test", StringComparison.OrdinalIgnoreCase));

            return drive.Id;
        }

        public async Task UploadFileAsync(string folderPath, string fileName, string csvContent)
        {
            var fileContent = Encoding.UTF8.GetBytes(csvContent);
            var driveId = await GetDriveIdAsync();

            var uploadPath = $"{folderPath}/{fileName}";
            using (var memoryStream = new MemoryStream(fileContent))
            {
                var uploadedFile = await graphServiceClient
                .Drives[driveId]
                .Items["root"]
                .ItemWithPath(uploadPath)
            .Content
                .PutAsync(memoryStream);
            }
        }
    }
}
