using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Dogs.ViewModels.Data.Models;
using DogsServer.Models;
using DogsServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using Strathweb.AspNetCore.AzureBlobFileProvider;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DogEventsController : BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork(new AppDbContext());
        CompositeFileProvider provider;

        public DogEventsController(CompositeFileProvider fileProvider)
        {
            provider = fileProvider;
        }



        [HttpPost]
        public IActionResult Add([FromBody]DogEventModel obj)
        {
            //dogs will be added to the event later
            var dogEvent = new DogEvent
            {
                DogId = obj.DogId,
                EventId = obj.EventId,
                DogTrackBlobUrl = obj.DogTrackBlobUrl,
                LostPerson = obj.LostPerson,
                LostPersonTrackBlobUrl = obj.LostPersonTrackBlobUrl,
                Notes = obj.Notes,
                Weather = obj.Weather
            };

            unitOfWork.DogEventRepository.Insert(dogEvent);
            unitOfWork.Commit();
            //var accessToken = HttpContext.GetTokenAsync("access_token").Result;

            return new ObjectResult(new { eventId = dogEvent.EventId, dogId = dogEvent.DogId });
        }

        [HttpGet]
        public List<DogEventModel> Get()
        {
            var dogEvents = unitOfWork.DogEventRepository.GetAll().ToList();
            var dogEventModels = new List<DogEventModel>();
            foreach (var dogEvent in dogEvents)
            {
                var dogEventModel = new DogEventModel
                {
                    DogId = dogEvent.DogId,
                    Dog = new DogModel
                    {
                        DogId = dogEvent.Dog.DogId,
                        Breed = dogEvent.Dog.Breed,
                        Name = dogEvent.Dog.Name,
                        GuideIdAndName = new IdNameModel {Id = dogEvent.Dog.Guide.GuideId, Name = $"{dogEvent.Dog.Guide.FirstName} {dogEvent.Dog.Guide.LastName}" }
                    },
                    EventId = dogEvent.EventId,
                    Event = new EventModel
                    {
                        City = dogEvent.Event.City,
                        Date = dogEvent.Event.Date,
                        Description = dogEvent.Event.Description,
                        EventId = dogEvent.Event.EventId,
                        Notes = dogEvent.Event.Notes,
                        StreetOrLocation = dogEvent.Event.StreetOrLocation,
                        Title = dogEvent.Event.Title
                    },
                    DogTrackBlobUrl = dogEvent.DogTrackBlobUrl,
                    LostPerson = dogEvent.LostPerson,
                    LostPersonTrackBlobUrl = dogEvent.LostPersonTrackBlobUrl,
                    Notes = dogEvent.Notes,
                    Weather = dogEvent.Weather
                    
                };
                dogEventModels.Add(dogEventModel);
            }
            return dogEventModels;

        }

        //api/DogEvents/DogEvent?dogId=5&eventId=1
        [HttpGet("DogEvent")]
        public DogEventModel Get(int dogId, int eventId)
        {
            //return unitOfWork.GuideRepository.GetById(id);
            var dogEvent = unitOfWork.DogEventRepository.GetByIds(dogId, eventId);
            var DogEventModel = new DogEventModel()
            {
                DogId = dogEvent.DogId,
                Dog = new DogModel
                {
                    DogId = dogEvent.Dog.DogId,
                    Breed = dogEvent.Dog.Breed,
                    Name = dogEvent.Dog.Name,
                    GuideIdAndName = new IdNameModel { Id = dogEvent.Dog.Guide.GuideId, Name = $"{dogEvent.Dog.Guide.FirstName} {dogEvent.Dog.Guide.LastName}" }
                },
                EventId = dogEvent.EventId,
                Event = new EventModel
                {
                    City = dogEvent.Event.City,
                    Date = dogEvent.Event.Date,
                    Description = dogEvent.Event.Description,
                    EventId = dogEvent.Event.EventId,
                    Notes = dogEvent.Event.Notes,
                    StreetOrLocation = dogEvent.Event.StreetOrLocation,
                    Title = dogEvent.Event.Title
                },
                DogTrackBlobUrl = dogEvent.DogTrackBlobUrl,
                LostPerson = dogEvent.LostPerson,
                LostPersonTrackBlobUrl = dogEvent.LostPersonTrackBlobUrl,
                Notes = dogEvent.Notes,
                Weather = dogEvent.Weather,
                
            };
            return DogEventModel;
        }

        [HttpPut("DogEvent")]
        public IActionResult Put(int dogId, int eventId, [FromBody]JObject obj)
        {
            try
            {
                var DogEvent = unitOfWork.DogEventRepository.GetByIds(dogId, eventId);
                var updatedEvent = obj.ToObject<DogEventModel>();
                //these never change - only the file behind that URL may change, but not the URL itself
                //DogEvent.DogTrackBlobUrl = updatedevent.DogTrackBlobUrl;
                //DogEvent.LostPersonTrackBlobUrl = updatedevent.LostPersonTrackBlobUrl;

                DogEvent.LostPerson = updatedEvent.LostPerson;
                DogEvent.Weather = updatedEvent.Weather;
                DogEvent.Notes = updatedEvent.Notes;
                
                unitOfWork.Commit();
                return new ObjectResult(new { eventId = DogEvent.EventId, dogId = DogEvent.DogId });
            }
            catch (Exception e)
            {
                return new ObjectResult(e.Message);
            }
        }

        [HttpDelete("DogEvent")]
        public IActionResult Delete(int dogId, int eventId)
        {
            unitOfWork.DogEventRepository.Delete(unitOfWork.DogEventRepository.GetByIds(dogId, eventId));
            unitOfWork.Commit();
            return new ObjectResult("event deleted successfully!");
        }

        [HttpPost("Upload")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
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
        //[DisableRequestSizeLimit]
        public HttpResponseMessage GetTrack(string filename)
        {
            var trackContents = provider.FileProviders.ToList()[0].GetDirectoryContents("");
            //IDirectoryContents contents = provider.GetDirectoryContents("/tracks");
            var fileList = trackContents.ToList();
            var requestedFile = fileList.Where(x => x.Name == filename).FirstOrDefault();
            var a = provider.FileProviders.ToList()[0].GetFileInfo("/" + filename);
            var stream = requestedFile.CreateReadStream();

            var reader = new StreamReader(stream);
            var stringStream = reader.ReadToEnd();


            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(stringStream) };
        }

        [HttpGet("GetAllByDogId")]
        public List<DogEventModel> GetAllByDogId(int dogId)
        {
            var dogEvents = unitOfWork.DogEventRepository.GetAllByDogId(dogId);
            var dogEventModels = new List<DogEventModel>();
            foreach (var dogEvent in dogEvents)
            {
                dogEventModels.Add(new DogEventModel
                {
                    DogId = dogEvent.DogId,
                    Dog = new DogModel
                    {
                        DogId = dogEvent.Dog.DogId,
                        Name = dogEvent.Dog.Name,
                        GuideIdAndName = new IdNameModel
                        {
                            Id = dogEvent.Dog.Guide.GuideId,
                            Name = $"{dogEvent.Dog.Guide.FirstName} {dogEvent.Dog.Guide.LastName}"
                        }
                    },
                    EventId = dogEvent.EventId,
                    Event = new EventModel
                    {
                        City = dogEvent.Event.City,
                        Date = dogEvent.Event.Date,
                        Description = dogEvent.Event.Description,
                        EventId = dogEvent.Event.EventId,
                        Notes = dogEvent.Event.Notes,
                        StreetOrLocation = dogEvent.Event.StreetOrLocation,
                        Title = dogEvent.Event.Title

                    },
                    LostPerson = dogEvent.LostPerson,
                    Weather = dogEvent.Weather,
                    Notes = dogEvent.Notes
                });
            }
            return dogEventModels;
        }

        [HttpGet("GetAllByEventId")]
        public List<DogEventModel> GetAllByEventId(int eventId)
        {
            var dogEvents = unitOfWork.DogEventRepository.GetAllByEventId(eventId);
            var dogEventModels = new List<DogEventModel>();
            foreach (var dogEvent in dogEvents)
            {
                dogEventModels.Add(new DogEventModel
                {
                    DogId = dogEvent.DogId,
                    Dog = new DogModel
                    {
                        DogId = dogEvent.Dog.DogId,
                        Name = dogEvent.Dog.Name,
                        GuideIdAndName = new IdNameModel
                        {
                            Id = dogEvent.Dog.Guide.GuideId,
                            Name = $"{dogEvent.Dog.Guide.FirstName} {dogEvent.Dog.Guide.LastName}"
                        }
                    },
                    EventId = dogEvent.EventId,
                    Event = new EventModel
                    {
                        City = dogEvent.Event.City,
                        Date = dogEvent.Event.Date,
                        Description = dogEvent.Event.Description,
                        EventId = dogEvent.Event.EventId,
                        Notes = dogEvent.Event.Notes,
                        StreetOrLocation = dogEvent.Event.StreetOrLocation,
                        Title = dogEvent.Event.Title

                    },
                    LostPerson = dogEvent.LostPerson,
                    Weather = dogEvent.Weather,
                    Notes = dogEvent.Notes
                });
            }
            return dogEventModels;
        }
    }
}