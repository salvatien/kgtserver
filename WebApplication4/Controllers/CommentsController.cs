﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dogs.Data.Models;
using Microsoft.EntityFrameworkCore;
using DogsServer.Repositories;
using Dogs.Data.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Dogs.Data.DbContexts;
using DogsServer.Services;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CommentsController : BaseController
    {
        private readonly UnitOfWork unitOfWork;
        private readonly AppDbContext appDbContext;

        public CommentsController(IUserService userService, AppDbContext dbContext) : base(userService)
        {
            appDbContext = dbContext;
            unitOfWork = new UnitOfWork(appDbContext);
        }

        [HttpGet("TrainingComments")]
        public List<CommentModel> GetAllTrainingComments()
        {
            var comments = unitOfWork.TrainingCommentRepository.GetAll().ToList();
            var CommentModels = new List<CommentModel>();
            foreach (var comment in comments)
            {
                var commentModel = new CommentModel
                {
                    CommentId = comment.TrainingCommentId,
                    AuthorId = comment.AuthorId,
                    AuthorName = $"{comment.Author.FirstName} {comment.Author.LastName}",
                    Content = comment.Content,
                    Date = comment.Date,
                    DogId = null,
                    TrainingId = comment.TrainingId

                };
                CommentModels.Add(commentModel);
            }
            return CommentModels;
        }

        [HttpGet("TrainingComment/{id}")]
        public CommentModel GetTrainingCommentById(int id)
        {
            var comment = unitOfWork.TrainingCommentRepository.GetById(id);
            var commentModel = new CommentModel
            {
                CommentId = comment.TrainingCommentId,
                AuthorId = comment.AuthorId,
                AuthorName = $"{comment.Author.FirstName} {comment.Author.LastName}",
                Content = comment.Content,
                Date = comment.Date,
                DogId = null,
                TrainingId = comment.TrainingId

            };
            return commentModel;
        }

        [HttpGet("TrainingCommentsByTrainingId/{trainingId}")]
        public List<CommentModel> GetAllTrainingCommentsByTrainingId(int trainingId)
        {
            var comments = unitOfWork.TrainingCommentRepository.GetAllByTrainingId(trainingId);
            var CommentModels = new List<CommentModel>();
            foreach (var comment in comments)
            {
                var commentModel = new CommentModel
                {
                    CommentId = comment.TrainingCommentId,
                    AuthorId = comment.AuthorId,
                    AuthorName = $"{comment.Author.FirstName} {comment.Author.LastName}",
                    Content = comment.Content,
                    Date = comment.Date,
                    DogId = null,
                    TrainingId = comment.TrainingId

                };
                CommentModels.Add(commentModel);
            }
            return CommentModels;
        }

        [HttpGet("DogTrainingComments")]
        public List<CommentModel> GetAllDogTrainingComments()
        {
            var comments = unitOfWork.DogTrainingCommentRepository.GetAll();
            var CommentModels = new List<CommentModel>();
            foreach (var comment in comments)
            {
                var commentModel = new CommentModel
                {
                    CommentId = comment.DogTrainingCommentId,
                    AuthorId = comment.AuthorId,
                    AuthorName = $"{comment.Author.FirstName} {comment.Author.LastName}",
                    Content = comment.Content,
                    Date = comment.Date,
                    DogId = comment.DogId,
                    TrainingId = comment.TrainingId

                };
                CommentModels.Add(commentModel);
            }
            return CommentModels;
        }

        [HttpGet("DogTrainingComment/{id}")]
        public CommentModel GetDogTrainingCommentById(int id)
        {
            var comment = unitOfWork.DogTrainingCommentRepository.GetById(id);
            
            var commentModel = new CommentModel
            {
                CommentId = comment.DogTrainingCommentId,
                AuthorId = comment.AuthorId,
                AuthorName = $"{comment.Author.FirstName} {comment.Author.LastName}",
                Content = comment.Content,
                Date = comment.Date,
                DogId = comment.DogId,
                TrainingId = comment.TrainingId

            };
            return commentModel;
        }

        [HttpGet("DogTrainingCommentsByDogIdAndTrainingId")]
        public List<CommentModel> GetDogTrainingCommentsByDogIdAndTrainingId(int dogId, int trainingId)
        {
            var comments = unitOfWork.DogTrainingCommentRepository.GetAllByDogIdAndTrainingId(dogId, trainingId);
            var CommentModels = new List<CommentModel>();
            foreach (var comment in comments)
            {
                var commentModel = new CommentModel
                {
                    CommentId = comment.DogTrainingCommentId,
                    AuthorId = comment.AuthorId,
                    AuthorName = $"{comment.Author.FirstName} {comment.Author.LastName}",
                    Content = comment.Content,
                    Date = comment.Date,
                    DogId = comment.DogId,
                    TrainingId = comment.TrainingId                    

                };
                CommentModels.Add(commentModel);
            }
            return CommentModels;
        }

        [HttpPost("TrainingComment")]
        public IActionResult AddNewTrainingComment([FromBody]CommentModel obj)
        {
            var comment = new TrainingComment
            {
                //TrainingCommentId = obj.CommentId,
                AuthorId = obj.AuthorId,
                Content = obj.Content,
                Date = obj.Date,
                TrainingId = obj.TrainingId,
            };
            unitOfWork.TrainingCommentRepository.Insert(comment);
            unitOfWork.Commit();
            return new ObjectResult(comment.TrainingCommentId);
        }

        [HttpPost("DogTrainingComment")]
        public IActionResult AddNewDogTrainingComment([FromBody]CommentModel obj)
        {
            var comment = new DogTrainingComment
            {
                //DogTrainingCommentId = obj.CommentId,
                AuthorId = obj.AuthorId,
                Content = obj.Content,
                Date = obj.Date,
                TrainingId = obj.TrainingId,
                DogId = obj.DogId.Value,
            };
            unitOfWork.DogTrainingCommentRepository.Insert(comment);
            unitOfWork.Commit();
            return new ObjectResult(comment.DogTrainingCommentId);
        }

        [HttpPost("TrainingComment/{id}")]
        public IActionResult UpdateTrainingComment(int id, [FromBody]CommentModel obj)
        {
            var comment = unitOfWork.TrainingCommentRepository.GetById(id);
            comment.TrainingCommentId = obj.CommentId;
            comment.AuthorId = obj.AuthorId;
            comment.Content = obj.Content;
            comment.Date = obj.Date;
            comment.TrainingId = obj.TrainingId;
            unitOfWork.Commit();
            return new ObjectResult(comment.TrainingCommentId);
        }

        [HttpPost("DogTrainingComment/{id}")]
        public IActionResult UpdateDogTrainingComment(int id, [FromBody]CommentModel obj)
        {
            var comment = unitOfWork.DogTrainingCommentRepository.GetById(id);
            comment.DogTrainingCommentId = obj.CommentId;
            comment.AuthorId = obj.AuthorId;
            comment.Content = obj.Content;
            comment.Date = obj.Date;
            comment.TrainingId = obj.TrainingId;
            comment.DogId = obj.DogId.Value;
            unitOfWork.Commit();
            return new ObjectResult(comment.DogTrainingCommentId);
        }
        
        [HttpDelete("TrainingComment/{id}")]
        public IActionResult DeleteTrainingComment(int id)
        {
            unitOfWork.TrainingCommentRepository.Delete(unitOfWork.TrainingCommentRepository.GetById(id));
            unitOfWork.Commit();
            return new ObjectResult("Comment deleted successfully!");
        }

        [HttpDelete("DogTrainingComment/{id}")]
        public IActionResult DeleteDogTrainingComment(int id)
        {
            unitOfWork.DogTrainingCommentRepository.Delete(unitOfWork.DogTrainingCommentRepository.GetById(id));
            unitOfWork.Commit();
            return new ObjectResult("Comment deleted successfully!");
        }
    }
}
