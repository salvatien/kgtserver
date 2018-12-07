using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DogsServer.Models;
using Microsoft.EntityFrameworkCore;
using DogsServer.Repositories;
using Dogs.ViewModels.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    public class GuidesController : BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork(new AppDbContext());

        [HttpGet]
        public List<Guide> Get()
        {
            return unitOfWork.GuideRepository.GetAll().ToList();
        }

        [HttpGet("{id}")]
        public Guide Get(int id)
        {
            return unitOfWork.GuideRepository.GetById(id);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]GuideModel obj)
        {
            var guide = unitOfWork.GuideRepository.GetById(id);
            var guideModel = obj;

            guide.Address = guideModel.Address;
            guide.City = guideModel.City;
            guide.FirstName = guideModel.FirstName;
            guide.GuideID = guideModel.GuideID;
            guide.LastName = guideModel.LastName;
            guide.Notes = guideModel.Notes;
            guide.Phone = guideModel.Phone;
            guide.Email = guideModel.Email;
            guide.IsAdmin = guideModel.IsAdmin;
            guide.IsMember = guideModel.IsMember;
            unitOfWork.Commit();
            return new ObjectResult("Guide modified successfully!");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
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
                IdentityId = GetCurrentUserIdentityId(),
                IsAdmin = false,
                IsMember = false,
                LastName = obj.LastName,
                Notes = obj.Notes,
                Phone = obj.Phone
            };
            unitOfWork.GuideRepository.Insert(guide);
            unitOfWork.Commit();
            var accessToken = HttpContext.GetTokenAsync("access_token").Result;

            return new ObjectResult(guide.GuideID);
        }
    }
}
