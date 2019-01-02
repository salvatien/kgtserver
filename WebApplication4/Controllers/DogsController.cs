using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DogsServer.Models;
using Microsoft.EntityFrameworkCore;
using DogsServer.Repositories;
using Newtonsoft.Json.Linq;
using Dogs.ViewModels.Data.Models;
using Dogs.ViewModels.Data.Models.Enums;
using DogsServer.Models.Enums;
using System.Net.Http;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Strathweb.AspNetCore.AzureBlobFileProvider;
using Microsoft.AspNetCore.Http.Internal;
using System.Diagnostics;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    public class DogsController : BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork(new AppDbContext());
        CompositeFileProvider provider;

        public DogsController(CompositeFileProvider fileProvider)
        {
            provider = fileProvider;
        }


        [HttpGet]
        public List<DogModel> Get()
        {
            var dogs = unitOfWork.DogRepository.GetAll().ToList();
            var dogModels = new List<DogModel>();
            foreach(var dog in dogs)
            {
                var dogModel = new DogModel
                {
                    DateOfBirth = dog.DateOfBirth,
                    DogId = dog.DogId,
                    Breed = dog.Breed,
                    GuideIdAndName = new IdNameModel
                    {
                        Id = dog.Guide != null ? dog.Guide.GuideId : 0,
                        Name = dog.Guide != null ? dog.Guide.FirstName + " " + dog.Guide.LastName : "Pies nie ma jeszcze przewodnika"
                    },
                    Level = dog.Level,
                    Name = dog.Name,
                    Notes = dog.Notes,
                    PhotoBlobUrl = dog.PhotoBlobUrl,
                    Workmodes = dog.Workmodes,
                    CertificateIds = dog.DogCertificates?.Select(x=>x.CertificateId).ToList(),
                    EventIds = dog.DogEvents?.Select(x=>x.EventId).ToList(),
                    TrainingIds = dog.DogTrainings?.Select(x=>x.TrainingId).ToList()
                };
                dogModels.Add(dogModel);
            }
            return dogModels;
        }

        [HttpGet("{id}")]
        public DogModel Get(int id)
        {
            var dog = unitOfWork.DogRepository.GetById(id);
            var dogModel = new DogModel
            {
                DateOfBirth = dog.DateOfBirth,
                DogId = dog.DogId,
                Breed = dog.Breed,
                GuideIdAndName = new IdNameModel
                {
                    Id = dog.Guide != null ? dog.Guide.GuideId : 0,
                    Name = dog.Guide != null ? dog.Guide.FirstName + " " + dog.Guide.LastName : "Pies nie ma jeszcze przewodnika"
                },
                Level = dog.Level,
                Name = dog.Name,
                Notes = dog.Notes,
                PhotoBlobUrl = dog.PhotoBlobUrl,
                Workmodes = dog.Workmodes,
                CertificateIds = dog.DogCertificates?.Select(x => x.CertificateId).ToList(),
                EventIds = dog.DogEvents?.Select(x => x.EventId).ToList(),
                TrainingIds = dog.DogTrainings?.Select(x => x.TrainingId).ToList()
            };
            return dogModel;
        }

        [HttpPost]
        public IActionResult Post([FromBody]JObject obj)
        {
            var dogModel = obj.ToObject<DogModel>();
            var guideId = dogModel.GuideIdAndName != null ? dogModel.GuideIdAndName.Id : 1;
            var guide = unitOfWork.GuideRepository.GetById(guideId);
            var dog = new Dog
            {
                DateOfBirth = dogModel.DateOfBirth,
                Guide = guide,
                Level = dogModel.Level,
                Name = dogModel.Name,
                Notes = dogModel.Notes,
                Workmodes = dogModel.Workmodes,
                Breed = dogModel.Breed,
                PhotoBlobUrl = dogModel.PhotoBlobUrl
            };
            
            unitOfWork.DogRepository.Insert(dog);
            unitOfWork.Commit();
            var x = dog.DogId;
            return new ObjectResult(x);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]JObject obj)
        {
            try
            {
                var dog = unitOfWork.DogRepository.GetById(id);
                var updatedDog = obj.ToObject<DogModel>();
                dog.Name = updatedDog.Name;
                dog.Breed = updatedDog.Breed;
                dog.DateOfBirth = updatedDog.DateOfBirth;
                dog.Level = updatedDog.Level;
                dog.Notes = updatedDog.Notes;
                dog.Workmodes = updatedDog.Workmodes;
                dog.PhotoBlobUrl = updatedDog.PhotoBlobUrl;
                if (updatedDog.GuideIdAndName != null)
                {
                    if (dog.Guide == null || dog.Guide.GuideId != updatedDog.GuideIdAndName.Id)
                    {
                        var guide = unitOfWork.GuideRepository.GetById(updatedDog.GuideIdAndName.Id);
                        dog.Guide = guide;
                    }
                }
                unitOfWork.Commit();
                return new ObjectResult(dog.DogId);
            }
            catch (Exception e)
            {
                return new ObjectResult(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            unitOfWork.DogRepository.Delete(unitOfWork.DogRepository.GetById(id));
            unitOfWork.Commit();
            return new ObjectResult("Dog deleted successfully!");
        }


        [HttpPost("Upload")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
            try {
                Request.EnableRewind();
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
                {
                    return Ok("UploadSuccess");
                }
                else
                    return BadRequest("UploadError");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
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
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("images");
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

        [HttpGet("getimage/{filename}")]
        //[DisableRequestSizeLimit]
        public HttpResponseMessage GetPhoto(string filename)
        {
            var imageContents = provider.FileProviders.ToList()[1].GetDirectoryContents("");
            //IDirectoryContents contents = provider.GetDirectoryContents("/tracks");
            var fileList = imageContents.ToList();
            var requestedFile = fileList.Where(x => x.Name == filename).FirstOrDefault();
            var a = provider.FileProviders.ToList()[1].GetFileInfo("/" + filename);
            var stream = requestedFile.CreateReadStream();

            var reader = new StreamReader(stream);
            var stringStream = reader.ReadToEnd();


            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(stringStream) };
        }
    }
}
