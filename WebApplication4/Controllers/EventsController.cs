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
    public class EventsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork(new AppDbContext());

        [HttpGet]
        public List<Event> Get()
        {
            return unitOfWork.EventRepository.GetAll().ToList();
        }

        [HttpGet("{id}")]
        public Event Get(int id)
        {
            return unitOfWork.EventRepository.GetById(id);
        }

        [HttpPost]
        public IActionResult Post([FromBody]Event obj)
        {
            unitOfWork.EventRepository.Insert(obj);
            unitOfWork.Commit();
            return new ObjectResult("Event added successfully!");
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Event obj)
        {
            var Event = unitOfWork.EventRepository.GetById(id);
            Event = obj;
            unitOfWork.Commit();
            return new ObjectResult("Event modified successfully!");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            unitOfWork.EventRepository.Delete(unitOfWork.EventRepository.GetById(id));
            unitOfWork.Commit();
            return new ObjectResult("Event deleted successfully!");
        }
    }
}
