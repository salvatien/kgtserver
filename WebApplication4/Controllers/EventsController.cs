using System.Collections.Generic;
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
    public class EventsController : BaseController
    {
        private readonly UnitOfWork unitOfWork;
        private readonly AppDbContext appDbContext;

        public EventsController(AppDbContext dbContext, IUserService userService) : base(userService)
        {
            appDbContext = dbContext;
            unitOfWork = new UnitOfWork(appDbContext);
        }

        [HttpGet]
        [AllowAnonymous]
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
                    Description = oneEvent.Description,
                    StreetOrLocation = oneEvent.StreetOrLocation,
                    Title = oneEvent.Title,
                    IsCommercialTraining = oneEvent.IsCommercialTraining,
                    GuideIds = oneEvent.GuideEvents?.Select(x => x.GuideId).ToList(),
                    DogIds = oneEvent.DogEvents?.Select(x=>x.DogId).ToList()
                };
                eventModels.Add(eventModel);
            }
            return eventModels;
        }
        [AllowAnonymous]
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
                Description = oneEvent.Description,
                StreetOrLocation = oneEvent.StreetOrLocation,
                Title = oneEvent.Title,
                IsCommercialTraining = oneEvent.IsCommercialTraining,
                GuideIds = oneEvent.GuideEvents?.Select(x => x.GuideId).ToList(),
                DogIds = oneEvent.DogEvents?.Select(x => x.DogId).ToList()
            };
            return eventModel;
        }

        [HttpPost]
        public IActionResult Post([FromBody]EventModel obj)
        {
            if (!UserService.IsCurrentUserAdmin(User) && !UserService.IsCurrentUserMember(User))
                return Forbid();
            var oneEvent = new Event
            {
                EventId = obj.EventId,
                City = obj.City,
                Date = obj.Date,
                Notes = obj.Notes,
                Description = obj.Description,
                StreetOrLocation = obj.StreetOrLocation,
                Title = obj.Title,
                IsCommercialTraining = obj.IsCommercialTraining
            };
            unitOfWork.EventRepository.Insert(oneEvent);
            unitOfWork.Commit();
            return new ObjectResult(oneEvent.EventId);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]EventModel obj)
        {
            if (!UserService.IsCurrentUserAdmin(User) && !UserService.IsCurrentUserMember(User))
                return Forbid();
            var oldEvent = unitOfWork.EventRepository.GetById(id);
            oldEvent.City = obj.City;
            oldEvent.Date = obj.Date;
            oldEvent.Notes = obj.Notes;
            oldEvent.Description = obj.Description;
            oldEvent.StreetOrLocation = obj.StreetOrLocation;
            oldEvent.Title = obj.Title;
            oldEvent.IsCommercialTraining = obj.IsCommercialTraining;
            unitOfWork.Commit();
            return new ObjectResult(oldEvent.EventId);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!UserService.IsCurrentUserAdmin(User) && !UserService.IsCurrentUserMember(User))
                return Forbid();
            unitOfWork.EventRepository.Delete(unitOfWork.EventRepository.GetById(id));
            unitOfWork.Commit();
            return new ObjectResult("Event deleted successfully!");
        }
    }
}
