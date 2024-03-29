﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DogsServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using DogsServer.Services;
using Dogs.Data.Models;
using Dogs.Data.DbContexts;
using Dogs.Data.DataTransferObjects;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class GuidesController : BaseController
    {
        private readonly UnitOfWork unitOfWork;
        private readonly AppDbContext appDbContext;

        public GuidesController(AppDbContext dbContext, IUserService userService) : base(userService)
        {
            appDbContext = dbContext;
            unitOfWork = new UnitOfWork(appDbContext);
        }

        [HttpGet]
        [AllowAnonymous]
        public List<GuideModel> Get()
        {
            var guides =  unitOfWork.GuideRepository.GetAll().ToList();
            var guideModelList = new List<GuideModel>();
            foreach (var g in guides)
            {
                var guideModel = new GuideModel {
                    GuideId = g.GuideId,
                    IdentityId = g.IdentityId,
                    FirstName = g.FirstName,
                    LastName = g.LastName,
                    Address = g.Address,
                    City = g.City,
                    Phone = g.Phone,
                    Email = g.Email,
                    Notes = g.Notes,
                    IsAdmin = g.IsAdmin,
                    IsMember = g.IsMember,
                    Dogs = g.Dogs.Select(d => new IdNameModel { Id = d.DogId, Name = d.Name }).ToList(),
                    EventIds = g.GuideEvents?.Select(e => e.EventId).ToList()
                };

                guideModelList.Add(guideModel);
            }
            return guideModelList;
        }

        [HttpGet("{id}")]
        public GuideModel Get(int id)
        {
            //return unitOfWork.GuideRepository.GetById(id);
            var g = unitOfWork.GuideRepository.GetById(id);
            var guideModel = new GuideModel()
            { 
                GuideId = g.GuideId,
                IdentityId = g.IdentityId,
                FirstName = g.FirstName,
                LastName = g.LastName,
                Address = g.Address,
                City = g.City,
                Phone = g.Phone,
                Email = g.Email,
                Notes = g.Notes,
                IsAdmin = g.IsAdmin,
                IsMember = g.IsMember,
                Dogs = g.Dogs.Select(d => new IdNameModel { Id = d.DogId, Name = d.Name }).ToList(),
                EventIds = g.GuideEvents?.Select(e=>e.EventId).ToList()
            };

            return guideModel;
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]GuideModel obj)
        {
            if (!UserService.IsCurrentUserAdmin(User) && id != UserService.GetCurrentUserId(User))
                return Forbid();
            var guide = unitOfWork.GuideRepository.GetById(id);
            var guideModel = obj;

            guide.Address = guideModel.Address;
            guide.City = guideModel.City;
            guide.Email = guideModel.Email;
            guide.FirstName = guideModel.FirstName;
            guide.LastName = guideModel.LastName;
            guide.Notes = guideModel.Notes;
            guide.Phone = guideModel.Phone;
            guide.IsAdmin = guideModel.IsAdmin;
            guide.IsMember = guideModel.IsMember;

            //var allDogs = unitOfWork.DogRepository.GetAll();
            //var dogIds = guideModel.Dogs.Select(x => x.Id).ToList();
            //var dogs = allDogs.Where(d => dogIds.Contains(d.DogID)).ToList();
            //guide.Dogs = dogs;

            unitOfWork.Commit();
            return new ObjectResult(guide.GuideId);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!UserService.IsCurrentUserAdmin(User))
                return Forbid();
            unitOfWork.GuideRepository.Delete(unitOfWork.GuideRepository.GetById(id));
            unitOfWork.Commit();
            return new ObjectResult("Guide deleted successfully!");
        }
        [Authorize]
        [HttpGet("getfreeguideid")]
        public int GetFreeGuideId()
        {
            return unitOfWork.GuideRepository.GetFreeId();
        }

        [Authorize]
        [HttpPost("register")]
        public IActionResult Register([FromBody]GuideModel obj)
        {
            var guide = new Guide
            {
                Address = obj.Address,
                City = obj.City,
                Email = obj.Email,
                FirstName = obj.FirstName,
                IdentityId = UserService.GetCurrentUserIdentityId(User),
                IsAdmin = false,
                IsMember = false,
                LastName = obj.LastName,
                Notes = obj.Notes,
                Phone = obj.Phone
            };
            unitOfWork.GuideRepository.Insert(guide);
            unitOfWork.Commit();
            //var accessToken = HttpContext.GetTokenAsync("access_token").Result;

            return new ObjectResult(guide.GuideId);
        }
    }
}
