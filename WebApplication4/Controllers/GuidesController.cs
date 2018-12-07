using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DogsServer.Models;
using Microsoft.EntityFrameworkCore;
using DogsServer.Repositories;
using Dogs.ViewModels.Data.Models;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    public class GuidesController : Controller
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

        [HttpPost]
        public IActionResult Post([FromBody]Guide obj)
        {
            unitOfWork.GuideRepository.Insert(obj);
            unitOfWork.Commit();
            return new ObjectResult("Guide added successfully!");
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]GuideModel obj)
        {
            /**/
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

            var allDogs = unitOfWork.DogRepository.GetAll();
            var dogIds = guideModel.Dogs.Select(x => x.Id).ToList();
            var dogs = allDogs.Where(d => dogIds.Contains(d.DogID)).ToList();
            guide.Dogs = dogs;

            unitOfWork.Commit();
            return new ObjectResult(guide.GuideID);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            unitOfWork.GuideRepository.Delete(unitOfWork.GuideRepository.GetById(id));
            unitOfWork.Commit();
            return new ObjectResult("Guide deleted successfully!");
        }
    }
}
