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
    public class DogTrainingsController : BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork(new AppDbContext());
        CompositeFileProvider provider;

        public DogTrainingsController(CompositeFileProvider fileProvider)
        {
            provider = fileProvider;
        }



        [HttpPost]
        public IActionResult Add([FromBody]DogTrainingModel obj)
        {
            if (!IsCurrentUserAdmin() && !IsCurrentUserMember())
                return Forbid();
            //dogs will be added to the training later
            var training = new DogTraining
            {
                Comments = null,
                DogId = obj.DogId,
                TrainingId = obj.TrainingId,
                DogTrackBlobUrl = obj.DogTrackBlobUrl,
                LostPerson = obj.LostPerson,
                LostPersonTrackBlobUrl = obj.LostPersonTrackBlobUrl,
                Notes = obj.Notes,
                Weather = obj.Weather,
                DelayTime = obj.DelayTime,
                LostPersonTrackLength = obj.LostPersonTrackLength
            };

            unitOfWork.DogTrainingRepository.Insert(training);
            unitOfWork.Commit();
            //var accessToken = HttpContext.GetTokenAsync("access_token").Result;

            return new ObjectResult(new { trainingId = training.TrainingId, dogId = training.DogId });
        }
        //api/dogtrainings/training?dogId=5&trainingId=1
        [HttpGet("Training")]
        public DogTrainingModel Get(int dogId, int trainingId)
        {
            if (!IsCurrentUserAdmin() && !IsCurrentUserMember())
                return null;
            //return unitOfWork.GuideRepository.GetById(id);
            var t = unitOfWork.DogTrainingRepository.GetByIds(dogId, trainingId);
            var dogTrainingModel = new DogTrainingModel()
            {
                DogId = t.DogId,
                TrainingId = t.TrainingId,
                DogTrackBlobUrl = t.DogTrackBlobUrl,
                LostPerson = t.LostPerson,
                LostPersonTrackBlobUrl = t.LostPersonTrackBlobUrl,
                Notes = t.Notes,
                Weather = t.Weather,
                DelayTime = t.DelayTime,
                LostPersonTrackLength = t.LostPersonTrackLength,
                Comments = t.Comments.Select(c => new CommentModel
                {
                    AuthorId = c.AuthorId,
                    AuthorName = c.Author.FirstName + " " + c.Author.LastName,
                    CommentId = c.DogTrainingCommentId,
                    Content = c.Content,
                    Date = c.Date
                }).ToList()
            };
            return dogTrainingModel;
        }

        [HttpPut("Training")]
        public IActionResult Put(int dogId, int trainingId, [FromBody]JObject obj)
        {
            try
            {
                if (!IsCurrentUserAdmin() && !IsCurrentUserMember())
                    return Forbid();
                var dogTraining = unitOfWork.DogTrainingRepository.GetByIds(dogId, trainingId);
                var updatedTraining = obj.ToObject<DogTrainingModel>();
                //these never change - only the file behind that URL may change, but not the URL itself
                //dogTraining.DogTrackBlobUrl = updatedTraining.DogTrackBlobUrl;
                //dogTraining.LostPersonTrackBlobUrl = updatedTraining.LostPersonTrackBlobUrl;

                dogTraining.LostPerson = updatedTraining.LostPerson;
                dogTraining.Weather = updatedTraining.Weather;
                dogTraining.Notes = updatedTraining.Notes;
                dogTraining.LostPersonTrackLength = updatedTraining.LostPersonTrackLength;
                dogTraining.DelayTime = updatedTraining.DelayTime;
                //TODO not sure if it should be updated here or not
                //dogTraining.Comments = updatedTraining.Comments.Select(c => new DogTrainingComment
                //{
                //    AuthorId = c.AuthorId,
                //    Author = unitOfWork.GuideRepository.GetById(c.AuthorId),
                //    DogTrainingCommentId = c.CommentId,
                //    Content = c.Content,
                //    Date = c.Date
                //}).ToList();
                unitOfWork.Commit();
                return new ObjectResult(new { trainingId = dogTraining.TrainingId, dogId = dogTraining.DogId });
            }
            catch (Exception e)
            {
                return new ObjectResult(e.Message);
            }
        }

        [HttpDelete("Training")]
        public IActionResult Delete(int dogId, int trainingId)
        {
            if (!IsCurrentUserAdmin() && !IsCurrentUserMember())
                return Forbid();
            unitOfWork.DogTrainingRepository.Delete(unitOfWork.DogTrainingRepository.GetByIds(dogId, trainingId));
            unitOfWork.Commit();
            return new ObjectResult("Training deleted successfully!");
        }

        [HttpPost("Upload")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
            if (!IsCurrentUserAdmin() && !IsCurrentUserMember())
                return Forbid();
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
            if (!IsCurrentUserAdmin() && !IsCurrentUserMember())
                return false;

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
            if (!IsCurrentUserAdmin() && !IsCurrentUserMember())
                return new HttpResponseMessage(HttpStatusCode.Forbidden);
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
        public List<DogTrainingModel> GetAllByDogId(int dogId)
        {
            if (!IsCurrentUserAdmin() && !IsCurrentUserMember())
                return null;
            //return unitOfWork.GuideRepository.GetById(id);
            var allDogTrainings = unitOfWork.DogTrainingRepository.GetAllByDogId(dogId);
            var dogTrainingModels = new List<DogTrainingModel>();
            foreach (var t in allDogTrainings)
            {
                dogTrainingModels.Add(new DogTrainingModel
                {
                    DogId = t.DogId,
                    Dog = new DogModel
                    {
                        Name = t.Dog.Name,
                        GuideIdAndName = new IdNameModel { Id = t.Dog.Guide.GuideId,
                            Name = t.Dog.Guide.FirstName + " " + t.Dog.Guide.LastName }
                    },
                    TrainingId = t.TrainingId,
                    Training = new TrainingModel
                    {
                        Date = t.Training.Date,
                        GeneralLocation = t.Training.GeneralLocation,
                        LocationDetails = t.Training.LocationDetails,
                        Notes = t.Training.Notes,
                        TrainingId = t.TrainingId
                    },
                    DogTrackBlobUrl = t.DogTrackBlobUrl,
                    LostPerson = t.LostPerson,
                    LostPersonTrackBlobUrl = t.LostPersonTrackBlobUrl,
                    Notes = t.Notes,
                    Weather = t.Weather,
                    DelayTime = t.DelayTime,
                    LostPersonTrackLength = t.LostPersonTrackLength
                    //,
                    //Comments = t.Comments.Select(c => new CommentModel
                    //{
                    //    AuthorId = c.AuthorId,
                    //    AuthorName = c.Author.FirstName + " " + c.Author.LastName,
                    //    CommentId = c.DogTrainingCommentId,
                    //    Content = c.Content,
                    //    Date = c.Date
                    //}).ToList()
                });
            }
            return dogTrainingModels;
        }

        [HttpGet("GetAllByTrainingId")]
        public List<DogTrainingModel> GetAllByTrainingId(int trainingId)
        {
            if (!IsCurrentUserAdmin() && !IsCurrentUserMember())
                return null;
            //return unitOfWork.GuideRepository.GetById(id);
            var allDogTrainings = unitOfWork.DogTrainingRepository.GetAllByTrainingId(trainingId);
            var dogTrainingModels = new List<DogTrainingModel>();
            foreach (var t in allDogTrainings)
            {
                dogTrainingModels.Add(new DogTrainingModel
                {
                    DogId = t.DogId,
                    Dog = new DogModel
                    {
                        DogId = t.Dog.DogId,
                        Name = t.Dog.Name,
                        GuideIdAndName = new IdNameModel
                        {
                            Id = t.Dog.Guide.GuideId,
                            Name = t.Dog.Guide.FirstName + " " + t.Dog.Guide.LastName
                        }
                    },
                    TrainingId = t.TrainingId,
                    Training = new TrainingModel
                    {
                        Date = t.Training.Date,
                        GeneralLocation = t.Training.GeneralLocation,
                        LocationDetails = t.Training.LocationDetails,
                        Notes = t.Training.Notes,
                        TrainingId = t.TrainingId
                    },
                    DogTrackBlobUrl = t.DogTrackBlobUrl,
                    LostPerson = t.LostPerson,
                    LostPersonTrackBlobUrl = t.LostPersonTrackBlobUrl,
                    Notes = t.Notes,
                    Weather = t.Weather,
                    DelayTime = t.DelayTime,
                    LostPersonTrackLength = t.LostPersonTrackLength
                    //,
                    //Comments = t.Comments.Select(c => new CommentModel
                    //{
                    //    AuthorId = c.AuthorId,
                    //    AuthorName = c.Author.FirstName + " " + c.Author.LastName,
                    //    CommentId = c.DogTrainingCommentId,
                    //    Content = c.Content,
                    //    Date = c.Date
                    //}).ToList()
                });
            }
            return dogTrainingModels;
        }
    }
}