using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Azure;

namespace DogsServer.Services
{
    public interface IBlobStorageService
    {
        public Task<bool> UploadToBlob(string filename, Stream stream, string blobContainerName);
    }

    public class BlobStorageService : IBlobStorageService
    {
        private readonly IConfiguration _configuration;
        public BlobStorageService(IUserService userService, IConfiguration configuration)
        {
            _configuration = configuration;
        }
        

        public async Task<bool> UploadToBlob(string filename, Stream stream, string blobContainerName)
        {
            string storageConnectionString = _configuration.GetConnectionString("BlobConnectionString");
            try
            {
                var containerClient = new BlobContainerClient(storageConnectionString, blobContainerName);
                await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

                if (stream != null)
                {
                    await UploadStream(containerClient, filename, stream);
                    return true;
                }
                return false;
            }
            catch (RequestFailedException ex)
            {
                //TODO logging
                return false;
            }
        }

        public static async Task UploadStream (BlobContainerClient containerClient, string fileName, Stream fileStream)
        {
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, true);
            fileStream.Close();
        }
    }
}
