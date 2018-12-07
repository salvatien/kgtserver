using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DogsServer.Models;
using Microsoft.EntityFrameworkCore;
using DogsServer.Repositories;
using Newtonsoft.Json.Linq;
using Dogs.ViewModels.Data.Models;
using Dogs.ViewModels.Data.Models.Enums;
using DogsServer.Models.Enums;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    public class DogsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork(new AppDbContext());

        [HttpGet]
        public List<DogModel> Get()
        {
            var dogs = unitOfWork.DogRepository.GetAll().ToList();
            var dogModels = new List<DogModel>();
            foreach(var dog in dogs)
            {
                var dogModel = new DogModel
                {
                    DateOfBirth = dog.DateOfBirth,
                    DogID = dog.DogID,
                    GuideIdAndName = new IdNameModel
                    {
                        Id = dog.Guide != null ? dog.Guide.GuideID : 0,
                        Name = dog.Guide != null ? dog.Guide.FirstName + " " + dog.Guide.LastName : "Pies nie ma jeszcze przewodnika"
                    },
                    Level = dog.Level,
                    Name = dog.Name,
                    Notes = dog.Notes,
                    Workmodes = dog.Workmodes
                };
                dogModels.Add(dogModel);
            }
            return dogModels;
        }

        [HttpGet("{id}")]
        public DogModel Get(int id)
        {
            var dog = unitOfWork.DogRepository.GetById(id);
            var dogModel = new DogModel
            {
                DateOfBirth = dog.DateOfBirth,
                DogID = dog.DogID,
                GuideIdAndName = new IdNameModel
                {
                    Id = dog.Guide != null ? dog.Guide.GuideID : 0,
                    Name = dog.Guide != null ? dog.Guide.FirstName + " " + dog.Guide.LastName : "Pies nie ma jeszcze przewodnika"
                },
                Level = dog.Level,
                Name = dog.Name,
                Notes = dog.Notes,
                Workmodes = dog.Workmodes
            };
            return dogModel;
        }

        [HttpPost]
        public IActionResult Post([FromBody]JObject obj)
        {
            var dogModel = obj.ToObject<DogModel>();
            var guideId = dogModel.GuideIdAndName != null ? dogModel.GuideIdAndName.Id : 1;
            var guide = unitOfWork.GuideRepository.GetById(guideId);
            var dog = new Dog
            {
                DateOfBirth = dogModel.DateOfBirth,
                Guide = guide,
                Level = dogModel.Level,
                Name = dogModel.Name,
                Notes = dogModel.Notes,
                Workmodes = dogModel.Workmodes
            };
            
            unitOfWork.DogRepository.Insert(dog);
            unitOfWork.Commit();
            var x = dog.DogID;
            return new ObjectResult(x);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]JObject obj)
        {
            try
            {
                var dog = unitOfWork.DogRepository.GetById(id);
                var updatedDog = obj.ToObject<DogModel>();
                dog.Name = updatedDog.Name;
                dog.DateOfBirth = updatedDog.DateOfBirth;
                dog.Level = updatedDog.Level;
                dog.Notes = updatedDog.Notes;
                dog.Workmodes = updatedDog.Workmodes;
                if (updatedDog.GuideIdAndName != null)
                {
                    if (dog.Guide == null || dog.Guide.GuideID != updatedDog.GuideIdAndName.Id)
                    {
                        var guide = unitOfWork.GuideRepository.GetById(updatedDog.GuideIdAndName.Id);
                        dog.Guide = guide;
                    }
                }
                unitOfWork.Commit();
                return new ObjectResult(dog.DogID);
            }
            catch (Exception e)
            {
                return new ObjectResult(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            unitOfWork.DogRepository.Delete(unitOfWork.DogRepository.GetById(id));
            unitOfWork.Commit();
            return new ObjectResult("Dog deleted successfully!");
        }
    }
}
