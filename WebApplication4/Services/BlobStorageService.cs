using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

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

            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);
                    await cloudBlobContainer.CreateIfNotExistsAsync();

                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);


                    if (stream != null)
                    {
                        //pass in memory stream directly
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        return false;
                    }

                    return true;
                }
                catch (StorageException ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
    }
}
