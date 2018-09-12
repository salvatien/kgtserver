using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DogsServer.Models;
using Microsoft.EntityFrameworkCore;
using DogsServer.Repositories;
using Newtonsoft.Json.Linq;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    public class DogsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork(new AppDbContext());

        [HttpGet]
        public List<Dog> Get()
        {
            return unitOfWork.DogRepository.GetAll().ToList();
        }

        [HttpGet("{id}")]
        public Dog Get(int id)
        {
            return unitOfWork.DogRepository.GetById(id);
        }

        [HttpPost]
        public IActionResult Post([FromBody]JObject obj)
        {
            var dog = obj.ToObject<Dog>();
            unitOfWork.DogRepository.Insert(dog);
            unitOfWork.Commit();
            return new ObjectResult("Dog added successfully!");
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]JObject obj)
        {
            var Dog = unitOfWork.DogRepository.GetById(id);
            var updatedDog = obj.ToObject<Dog>();
            Dog.Name = updatedDog.Name;
            //Dog.Guide = updatedDog.Guide; //THIS SHOULD NOT BE COMMENTED BUT NOW CLIENT SENDS NEW GUIDE() WHICH GENERATES ERRORS
            Dog.DateOfBirth = updatedDog.DateOfBirth;
            Dog.Level = updatedDog.Level;
            Dog.Notes = updatedDog.Notes;
            Dog.Workmode = updatedDog.Workmode;
            unitOfWork.Commit();
            return new ObjectResult("Dog modified successfully!");
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
