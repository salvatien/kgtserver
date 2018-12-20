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
    public class TrainingsController : BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork(new AppDbContext());


        [HttpPost]
        public IActionResult Add([FromBody]TrainingModel obj)
        {
            //dogs will be added to the training later
            var training = new Training
            {
                Comments = null,
                Date = obj.Date,
                GeneralLocation = obj.GeneralLocation,
                LocationDetails = obj.LocationDetails,
                Notes = obj.Notes,

            };
            
            unitOfWork.TrainingRepository.Insert(training);
            unitOfWork.Commit();
            //var accessToken = HttpContext.GetTokenAsync("access_token").Result;

            return new ObjectResult(training.TrainingId);
        }

        [HttpGet("{id}")]
        public TrainingModel Get(int id)
        {
            //return unitOfWork.GuideRepository.GetById(id);
            var t = unitOfWork.TrainingRepository.GetById(id);
            var trainingModel = new TrainingModel()
            {
                Date = t.Date,
                GeneralLocation = t.GeneralLocation,
                LocationDetails = t.LocationDetails,
                Notes = t.Notes,
                Comments = t.Comments.Select(c => new CommentModel
                    {
                        AuthorId = c.AuthorId,
                        AuthorName = c.Author.FirstName + " " + c.Author.LastName,
                        CommentId = c.TrainingCommentId,
                        Content = c.Content,
                        Date = c.Date
                    }).ToList()
            };
            return trainingModel;
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]JObject obj)
        {
            try
            {
                var training = unitOfWork.TrainingRepository.GetById(id);
                var updatedTraining = obj.ToObject<TrainingModel>();
                training.Date = updatedTraining.Date;
                training.GeneralLocation = updatedTraining.GeneralLocation;
                training.LocationDetails = updatedTraining.LocationDetails;
                training.Notes = updatedTraining.Notes;
                training.Comments = updatedTraining.Comments.Select(c => new TrainingComment
                {
                    AuthorId = c.AuthorId,
                    Author = unitOfWork.GuideRepository.GetById(c.AuthorId),
                    TrainingCommentId = c.CommentId,
                    Content = c.Content,
                    Date = c.Date
                }).ToList();
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
            unitOfWork.TrainingRepository.Delete(unitOfWork.TrainingRepository.GetById(id));
            unitOfWork.Commit();
            return new ObjectResult("Training deleted successfully!");
        }
    }
}