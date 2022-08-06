using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogsServer.Repositories;
using DogsServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Dogs.Data.Models;
using Dogs.Data.DbContexts;
using Dogs.Data.DataTransferObjects;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DogTrainingsController : BlobUploaderControllerBase
    {
        private readonly UnitOfWork unitOfWork;
        private readonly AppDbContext appDbContext;

        public DogTrainingsController(AppDbContext dbContext, IUserService userService, IBlobStorageService blobStorageService) 
            : base(userService, blobStorageService)
        {
            appDbContext = dbContext;
            unitOfWork = new UnitOfWork(appDbContext);
        }

        [HttpPost]
        public IActionResult Add([FromBody]DogTrainingModel obj)
        {
            if (!UserService.IsCurrentUserAdmin(User) && !UserService.IsCurrentUserMember(User))
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
                GroundType = obj.GroundType,
                DelayTime = obj.DelayTime,
                LostPersonTrackLength = obj.LostPersonTrackLength,
                AdditionalPictureBlobUrl = obj.AdditionalPictureBlobUrl
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
            if (!UserService.IsCurrentUserAdmin(User) && !UserService.IsCurrentUserMember(User))
                return null;
            //return unitOfWork.GuideRepository.GetById(id);
            var t = unitOfWork.DogTrainingRepository.GetByIds(dogId, trainingId);
            if(String.IsNullOrWhiteSpace(t.Weather))
            {
                var training = unitOfWork.TrainingRepository.GetById(trainingId);
                t.Weather = training.Weather;
            }
            var dogTrainingModel = new DogTrainingModel()
            {
                DogId = t.DogId,
                TrainingId = t.TrainingId,
                DogTrackBlobUrl = t.DogTrackBlobUrl,
                LostPerson = t.LostPerson,
                LostPersonTrackBlobUrl = t.LostPersonTrackBlobUrl,
                Notes = t.Notes,
                Weather = t.Weather,
                GroundType = t.GroundType,
                DelayTime = t.DelayTime,
                LostPersonTrackLength = t.LostPersonTrackLength,
                AdditionalPictureBlobUrl = t.AdditionalPictureBlobUrl,
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
                if (!UserService.IsCurrentUserAdmin(User) && !UserService.IsCurrentUserMember(User))
                    return Forbid();
                var dogTraining = unitOfWork.DogTrainingRepository.GetByIds(dogId, trainingId);
                var updatedTraining = obj.ToObject<DogTrainingModel>();
                //these never change - only the file behind that URL may change, but not the URL itself
                //dogTraining.DogTrackBlobUrl = updatedTraining.DogTrackBlobUrl;
                //dogTraining.LostPersonTrackBlobUrl = updatedTraining.LostPersonTrackBlobUrl;

                dogTraining.LostPerson = updatedTraining.LostPerson;
                if(updatedTraining.Weather != dogTraining.Training.Weather)
                    dogTraining.Weather = updatedTraining.Weather;
                dogTraining.Notes = updatedTraining.Notes;
                dogTraining.GroundType = updatedTraining.GroundType;
                dogTraining.LostPersonTrackLength = updatedTraining.LostPersonTrackLength;
                dogTraining.DelayTime = updatedTraining.DelayTime;
                dogTraining.AdditionalPictureBlobUrl = updatedTraining.AdditionalPictureBlobUrl;
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
            if (!UserService.IsCurrentUserAdmin(User) && !UserService.IsCurrentUserMember(User))
                return Forbid();
            foreach (var comment in unitOfWork.DogTrainingCommentRepository.GetAllByDogIdAndTrainingId(dogId, trainingId))
                unitOfWork.DogTrainingCommentRepository.Delete(comment);
            unitOfWork.DogTrainingRepository.Delete(unitOfWork.DogTrainingRepository.GetByIds(dogId, trainingId));
            unitOfWork.Commit();
            return new ObjectResult("Training deleted successfully!");
        }

        [HttpPost("UploadImage")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadImage()
        {
            if (!UserService.IsCurrentUserAdmin(User) && !UserService.IsCurrentUserMember(User))
                return Forbid();
            return await Upload("images");
        }


        [HttpPost("Upload")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadTracks()
        {
            if (!UserService.IsCurrentUserAdmin(User) && !UserService.IsCurrentUserMember(User))
                return Forbid();
            return await Upload("tracks");
        }

        [HttpGet("GetAllByDogId")]
        public List<DogTrainingModel> GetAllByDogId(int dogId)
        {
            if (!UserService.IsCurrentUserAdmin(User) && !UserService.IsCurrentUserMember(User))
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
                        TrainingId = t.TrainingId,
                        Weather = t.Weather
                    },
                    DogTrackBlobUrl = t.DogTrackBlobUrl,
                    LostPerson = t.LostPerson,
                    LostPersonTrackBlobUrl = t.LostPersonTrackBlobUrl,
                    Notes = t.Notes,
                    GroundType = t.GroundType,
                    Weather = t.Weather ?? t.Training.Weather,
                    DelayTime = t.DelayTime,
                    LostPersonTrackLength = t.LostPersonTrackLength,
                    AdditionalPictureBlobUrl = t.AdditionalPictureBlobUrl
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
            if (!UserService.IsCurrentUserAdmin(User) && !UserService.IsCurrentUserMember(User))
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
                        TrainingId = t.TrainingId,
                        Weather = t.Weather
                    },
                    DogTrackBlobUrl = t.DogTrackBlobUrl,
                    LostPerson = t.LostPerson,
                    LostPersonTrackBlobUrl = t.LostPersonTrackBlobUrl,
                    Notes = t.Notes,
                    GroundType = t.GroundType,
                    Weather = t.Weather ?? t.Training.Weather,
                    DelayTime = t.DelayTime,
                    LostPersonTrackLength = t.LostPersonTrackLength,
                    AdditionalPictureBlobUrl = t.AdditionalPictureBlobUrl
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