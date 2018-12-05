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
            var guide = unitOfWork.GuideRepository.GetById(id);
            var guideModel = obj;

            guide.Address = guideModel.Address;
            guide.City = guideModel.City;
            guide.DateOfBirth = guideModel.DateOfBirth;
            guide.FirstName = guideModel.FirstName;
            guide.Fitness = guideModel.Fitness;
            guide.GuideID = guideModel.GuideID;
            guide.LastName = guideModel.LastName;
            guide.Notes = guideModel.Notes;
            guide.Phone = guideModel.Phone;

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
    }
}
