using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DogsServer.Models;
using Microsoft.EntityFrameworkCore;
using DogsServer.Repositories;


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
        public IActionResult Post([FromBody]Dog obj)
        {
            unitOfWork.DogRepository.Insert(obj);
            unitOfWork.Commit();
            return new ObjectResult("Dog added successfully!");
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Dog obj)
        {
            var Dog = unitOfWork.DogRepository.GetById(id);
            Dog = obj;
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
