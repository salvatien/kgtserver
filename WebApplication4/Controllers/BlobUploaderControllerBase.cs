using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Net;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.AspNetCore.Http;
using DogsServer.DbContexts;

namespace DogsServer.Controllers
{
    public abstract class BlobUploaderControllerBase : BaseController
    {
        private readonly AppDbContext appDbContext;

        public BlobUploaderControllerBase(AppDbContext dbContext) : base(dbContext)
        {
            appDbContext = dbContext;
        }
        protected async Task<IActionResult> Upload(string blobContainerName)
        {
            try
            {
                Request.EnableBuffering();
                var uploadSuccess = false;
                if (Request.Form.Files.Count > 0)
                {
                    foreach (var formFile in Request.Form.Files)
                    {
                        if (formFile.Length <= 0)
                        {
                            continue;
                        }
                        //read directly from stream for blob upload      
                        using (var stream = formFile.OpenReadStream())
                        {
                            uploadSuccess = await UploadToBlob(formFile.FileName, stream, blobContainerName);
                        }
                    }
                }

                if (uploadSuccess)
                {
                    return Ok("UploadSuccess");
                }
                else
                    return BadRequest("UploadError");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private async Task<bool> UploadToBlob(string filename, Stream stream, string blobContainerName)
        {
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=kgtstorage;AccountKey=PcFA7+GInK3Q/tqsavRf6tyGD0p8b2dsh7V2CsqKHukZsDyvIKuUBMK4XWhB+ygQbT23pJuXfIbDPJfh7EpQGw==;EndpointSuffix=core.windows.net";

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    // Create a container called 'uploadblob' and append a GUID value to it to make the name unique. 
                    cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);
                    await cloudBlobContainer.CreateIfNotExistsAsync();

                    // Set the permissions so the blobs are public. 
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
                finally
                {
                    // OPTIONAL: Clean up resources, e.g. blob container
                    //if (cloudBlobContainer != null)
                    //{
                    //    await cloudBlobContainer.DeleteIfExistsAsync();
                    //}
                }
            }
            else
            {
                return false;
            }

        }

        protected HttpResponseMessage GetFile(IFileProvider fileProvider, string filename)
        {
            var fileContents = fileProvider.GetDirectoryContents("");
            var fileList = fileContents.ToList();
            var requestedFile = fileList.Where(x => x.Name == filename).FirstOrDefault();
            var stream = requestedFile.CreateReadStream();

            var reader = new StreamReader(stream);
            var stringStream = reader.ReadToEnd();

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(stringStream) };
        }
    }
}
