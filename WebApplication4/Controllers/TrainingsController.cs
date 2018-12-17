using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Strathweb.AspNetCore.AzureBlobFileProvider;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class TrainingsController : ControllerBase
    {

        CompositeFileProvider provider;

        public TrainingsController(CompositeFileProvider fileProvider)
        {
            provider = fileProvider;
        }
        [HttpPost("uploadFile")]
        [DisableRequestSizeLimit]
        public HttpResponseMessage UploadFile()
        {

            HttpResponseMessage result = null;
            if (Request.Form.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (var postedFile in Request.Form.Files)
                {
                    Console.WriteLine(postedFile.FileName);
                }
                //result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
            }
            else
            {
                //result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return result;
        }

        [HttpPost("Upload")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
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
                        uploadSuccess = await UploadToBlob(formFile.FileName, stream);
                    }
                }

            }

            if (uploadSuccess)
                return Ok("UploadSuccess");
            else
                return BadRequest("UploadError");
        }

        private async Task<bool> UploadToBlob(string filename, Stream stream)
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
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("tracks");
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

        [HttpGet("gettrack/{filename}")]
        [DisableRequestSizeLimit]
        public FileStreamResult GetTrack(string filename)
        {
            var trackContents = provider.FileProviders.ToList()[0].GetDirectoryContents("");
            //IDirectoryContents contents = provider.GetDirectoryContents("/tracks");
            var fileList = trackContents.ToList();
            var requestedFile = fileList.Where(x => x.Name == filename).FirstOrDefault();
            var stream = requestedFile.CreateReadStream();
            return File(stream, "image/jpeg");
        }
    }
}