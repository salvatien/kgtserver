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
    public class TrainingsController : BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork(new AppDbContext());


        [HttpPost]
        public IActionResult Add([FromBody]TrainingModel obj)
        {
            if (!IsCurrentUserAdmin() && !IsCurrentUserMember())
                return Forbid();
            //dogs will be added to the training later
            var training = new Training
            {
                Comments = new List<TrainingComment>(),
                Date = obj.Date,
                GeneralLocation = obj.GeneralLocation,
                LocationDetails = obj.LocationDetails,
                Notes = obj.Notes,
                Weather = obj.Weather,
                DogTrainings = new List<DogTraining>()

            };
            
            unitOfWork.TrainingRepository.Insert(training);
            unitOfWork.Commit();
            //var accessToken = HttpContext.GetTokenAsync("access_token").Result;

            return new ObjectResult(training.TrainingId);
        }

        [HttpGet]
        public List<TrainingModel> Get()
        {
            if (!IsCurrentUserAdmin() && !IsCurrentUserMember())
                return null;
            var trainings = unitOfWork.TrainingRepository.GetAll().ToList();
            var trainingModelList = new List<TrainingModel>();
            foreach (var t in trainings)
            {
                var trainingModel = new TrainingModel()
                {
                    TrainingId = t.TrainingId,
                    Date = t.Date,
                    GeneralLocation = t.GeneralLocation,
                    LocationDetails = t.LocationDetails,
                    Notes = t.Notes,
                    Weather = t.Weather,
                    Comments = (t.Comments == null || !t.Comments.Any()) 
                        ? new List <CommentModel> () 
                        : t.Comments.Select(c => new CommentModel
                        {
                            AuthorId = c.AuthorId,
                            AuthorName = c.Author.FirstName + " " + c.Author.LastName,
                            CommentId = c.TrainingCommentId,
                            Content = c.Content,
                            Date = c.Date
                        }).ToList(),
                    DogTrainings = (t.DogTrainings== null || !t.DogTrainings.Any())
                        ? new List<DogTrainingModel>()
                        : t.DogTrainings.Select(dt => new DogTrainingModel
                    {
                        DogId = dt.DogId,
                        TrainingId = dt.TrainingId,
                        Dog = new DogModel
                        {
                            Name = dt.Dog.Name,
                            GuideIdAndName = new IdNameModel
                            {
                                Id = dt.Dog.Guide.GuideId,
                                Name = dt.Dog.Guide.FirstName + " " + dt.Dog.Guide.LastName
                            }
                        },
                        LostPerson = dt.LostPerson
                    }).ToList()
                };

                trainingModelList.Add(trainingModel);
            }
            return trainingModelList;
        }

        [HttpGet("{id}")]
        public TrainingModel Get(int id)
        {
            if (!IsCurrentUserAdmin() && !IsCurrentUserMember())
                return null;
            //return unitOfWork.GuideRepository.GetById(id);
            var t = unitOfWork.TrainingRepository.GetById(id);
            var trainingModel = new TrainingModel()
            {
                TrainingId = t.TrainingId,
                Date = t.Date,
                GeneralLocation = t.GeneralLocation,
                LocationDetails = t.LocationDetails,
                Notes = t.Notes,
                Weather = t.Weather,
                Comments = (t.Comments == null || !t.Comments.Any())
                        ? new List<CommentModel>()
                        : t.Comments.Select(c => new CommentModel
                {
                    AuthorId = c.AuthorId,
                    AuthorName = c.Author.FirstName + " " + c.Author.LastName,
                    CommentId = c.TrainingCommentId,
                    Content = c.Content,
                    Date = c.Date
                }).ToList(),
                DogTrainings = (t.DogTrainings == null || !t.DogTrainings.Any())
                        ? new List<DogTrainingModel>()
                        : t.DogTrainings.Select(dt => new DogTrainingModel
                {
                    DogId = dt.DogId,
                    TrainingId = dt.TrainingId,
                    Dog = new DogModel
                    {
                        Name = dt.Dog.Name,
                        GuideIdAndName = new IdNameModel
                        {
                            Id = dt.Dog.Guide.GuideId,
                            Name = dt.Dog.Guide.FirstName + " " + dt.Dog.Guide.LastName
                        }
                    },
                    LostPerson = dt.LostPerson
                }).ToList()
            };
            return trainingModel;
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]JObject obj)
        {
            if (!IsCurrentUserAdmin() && !IsCurrentUserMember())
                return Forbid();
            try
            {
                var training = unitOfWork.TrainingRepository.GetById(id);
                var updatedTraining = obj.ToObject<TrainingModel>();
                training.Date = updatedTraining.Date;
                training.GeneralLocation = updatedTraining.GeneralLocation;
                training.LocationDetails = updatedTraining.LocationDetails;
                training.Notes = updatedTraining.Notes;
                training.Weather = updatedTraining.Weather;
                //updating comments and dog trainings is not possible through update training
                unitOfWork.Commit();
                return new ObjectResult(training.TrainingId);
            }
            catch (Exception e)
            {
                return new ObjectResult(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!IsCurrentUserAdmin())
                return Forbid();
            unitOfWork.TrainingRepository.Delete(unitOfWork.TrainingRepository.GetById(id));
            unitOfWork.Commit();
            return new ObjectResult("Training deleted successfully!");
        }
    }
}