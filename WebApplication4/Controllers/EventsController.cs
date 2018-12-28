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
    public class EventsController : BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork(new AppDbContext());

        [HttpGet]
        public List<EventModel> Get()
        {
            var events = unitOfWork.EventRepository.GetAll().ToList();
            var eventModels = new List<EventModel>();
            foreach (var oneEvent in events)
            {
                var eventModel = new EventModel
                {
                    EventId = oneEvent.EventId,
                    City = oneEvent.City,
                    Date = oneEvent.Date,
                    Notes = oneEvent.Notes,
                    StreetOrLocation = oneEvent.StreetOrLocation,
                    Title = oneEvent.Title,
                    GuideIds = oneEvent.GuideEvents.Select(x => x.GuideId).ToList()
                };
                eventModels.Add(eventModel);
            }
            return eventModels;
        }

        [HttpGet("{id}")]
        public EventModel Get(int id)
        {
            var oneEvent = unitOfWork.EventRepository.GetById(id);
            var eventModel = new EventModel
            {
                EventId = oneEvent.EventId,
                City = oneEvent.City,
                Date = oneEvent.Date,
                Notes = oneEvent.Notes,
                StreetOrLocation = oneEvent.StreetOrLocation,
                Title = oneEvent.Title,
                GuideIds = oneEvent.GuideEvents.Select(x => x.GuideId).ToList()
            };
            return eventModel;
        }

        [HttpPost]
        public IActionResult Post([FromBody]EventModel obj)
        {
            var oneEvent = new Event
            {
                EventId = obj.EventId,
                City = obj.City,
                Date = obj.Date,
                Notes = obj.Notes,
                Description = obj.Description,
                StreetOrLocation = obj.StreetOrLocation,
                Title = obj.Title,
            };
            unitOfWork.EventRepository.Insert(oneEvent);
            unitOfWork.Commit();
            return new ObjectResult(oneEvent.EventId);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]EventModel obj)
        {
            var oldEvent = unitOfWork.EventRepository.GetById(id);
            oldEvent.City = obj.City;
            oldEvent.Date = obj.Date;
            oldEvent.Notes = obj.Notes;
            oldEvent.Description = obj.Description;
            oldEvent.StreetOrLocation = obj.StreetOrLocation;
            oldEvent.Title = obj.Title;
            unitOfWork.Commit();
            return new ObjectResult(oldEvent.EventId);
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
