﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogsServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using DogsServer.Services;
using Dogs.Data.Models;
using Dogs.Data.DbContexts;
using Dogs.Data.DataTransferObjects;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DogEventsController : BlobUploaderControllerBase
    {
        private readonly UnitOfWork unitOfWork;
        private readonly AppDbContext appDbContext;

        public DogEventsController(AppDbContext dbContext, IUserService userService, IBlobStorageService blobStorageService)
            : base(userService, blobStorageService)
        {
            appDbContext = dbContext;
            unitOfWork = new UnitOfWork(appDbContext);
        }
        
        [HttpPost]
        public IActionResult Add([FromBody]DogEventModel obj)
        {
            if (!UserService.IsCurrentUserAdmin(User) && !UserService.IsCurrentUserMember(User))
                return Forbid();
            //dogs will be added to the event later
            var dogEvent = new DogEvent
            {
                DogId = obj.DogId,
                EventId = obj.EventId,
                DogTrackBlobUrl = obj.DogTrackBlobUrl,
                LostPerson = obj.LostPerson,
                LostPersonTrackBlobUrl = obj.LostPersonTrackBlobUrl,
                Notes = obj.Notes,
                Weather = obj.Weather
            };

            unitOfWork.DogEventRepository.Insert(dogEvent);
            unitOfWork.Commit();
            //var accessToken = HttpContext.GetTokenAsync("access_token").Result;

            return new ObjectResult(new { eventId = dogEvent.EventId, dogId = dogEvent.DogId });
        }

        [HttpGet]
        public List<DogEventModel> Get()
        {
            var dogEvents = unitOfWork.DogEventRepository.GetAll().ToList();
            var dogEventModels = new List<DogEventModel>();
            foreach (var dogEvent in dogEvents)
            {
                var dogEventModel = new DogEventModel
                {
                    DogId = dogEvent.DogId,
                    Dog = new DogModel
                    {
                        DogId = dogEvent.Dog.DogId,
                        Breed = dogEvent.Dog.Breed,
                        Name = dogEvent.Dog.Name,
                        GuideIdAndName = new IdNameModel {Id = dogEvent.Dog.Guide.GuideId, Name = $"{dogEvent.Dog.Guide.FirstName} {dogEvent.Dog.Guide.LastName}" }
                    },
                    EventId = dogEvent.EventId,
                    Event = new EventModel
                    {
                        City = dogEvent.Event.City,
                        Date = dogEvent.Event.Date,
                        Description = dogEvent.Event.Description,
                        EventId = dogEvent.Event.EventId,
                        Notes = dogEvent.Event.Notes,
                        StreetOrLocation = dogEvent.Event.StreetOrLocation,
                        Title = dogEvent.Event.Title
                    },
                    DogTrackBlobUrl = dogEvent.DogTrackBlobUrl,
                    LostPerson = dogEvent.LostPerson,
                    LostPersonTrackBlobUrl = dogEvent.LostPersonTrackBlobUrl,
                    Notes = dogEvent.Notes,
                    Weather = dogEvent.Weather
                    
                };
                dogEventModels.Add(dogEventModel);
            }
            return dogEventModels;

        }

        //api/DogEvents/DogEvent?dogId=5&eventId=1
        [HttpGet("DogEvent")]
        public DogEventModel Get(int dogId, int eventId)
        {
            //return unitOfWork.GuideRepository.GetById(id);
            var dogEvent = unitOfWork.DogEventRepository.GetByIds(dogId, eventId);
            var DogEventModel = new DogEventModel()
            {
                DogId = dogEvent.DogId,
                Dog = new DogModel
                {
                    DogId = dogEvent.Dog.DogId,
                    Breed = dogEvent.Dog.Breed,
                    Name = dogEvent.Dog.Name,
                    GuideIdAndName = new IdNameModel { Id = dogEvent.Dog.Guide.GuideId, Name = $"{dogEvent.Dog.Guide.FirstName} {dogEvent.Dog.Guide.LastName}" }
                },
                EventId = dogEvent.EventId,
                Event = new EventModel
                {
                    City = dogEvent.Event.City,
                    Date = dogEvent.Event.Date,
                    Description = dogEvent.Event.Description,
                    EventId = dogEvent.Event.EventId,
                    Notes = dogEvent.Event.Notes,
                    StreetOrLocation = dogEvent.Event.StreetOrLocation,
                    Title = dogEvent.Event.Title
                },
                DogTrackBlobUrl = dogEvent.DogTrackBlobUrl,
                LostPerson = dogEvent.LostPerson,
                LostPersonTrackBlobUrl = dogEvent.LostPersonTrackBlobUrl,
                Notes = dogEvent.Notes,
                Weather = dogEvent.Weather,
                
            };
            return DogEventModel;
        }

        [HttpPut("DogEvent")]
        public IActionResult Put(int dogId, int eventId, [FromBody]JObject obj)
        {
            try
            {
                var DogEvent = unitOfWork.DogEventRepository.GetByIds(dogId, eventId);
                var updatedEvent = obj.ToObject<DogEventModel>();
                //these never change - only the file behind that URL may change, but not the URL itself
                //DogEvent.DogTrackBlobUrl = updatedevent.DogTrackBlobUrl;
                //DogEvent.LostPersonTrackBlobUrl = updatedevent.LostPersonTrackBlobUrl;

                DogEvent.LostPerson = updatedEvent.LostPerson;
                DogEvent.Weather = updatedEvent.Weather;
                DogEvent.Notes = updatedEvent.Notes;
                
                unitOfWork.Commit();
                return new ObjectResult(new { eventId = DogEvent.EventId, dogId = DogEvent.DogId });
            }
            catch (Exception e)
            {
                return new ObjectResult(e.Message);
            }
        }

        [HttpDelete("DogEvent")]
        public IActionResult Delete(int dogId, int eventId)
        {
            if (!UserService.IsCurrentUserAdmin(User) && !UserService.IsCurrentUserMember(User))
                return Forbid();
            unitOfWork.DogEventRepository.Delete(unitOfWork.DogEventRepository.GetByIds(dogId, eventId));
            unitOfWork.Commit();
            return new ObjectResult("event deleted successfully!");
        }

        [HttpPost("UploadImage")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadImage()
        {
            if (!UserService.IsCurrentUserAdmin(User) && !UserService.IsCurrentUserMember(User))
                return Forbid();
            return await Upload("images");
        }


        [HttpPost("Upload")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadTracks()
        {
            if (!UserService.IsCurrentUserAdmin(User) && !UserService.IsCurrentUserMember(User))
                return Forbid();
            return await Upload("tracks");
        }

        
        [HttpGet("GetAllByDogId")]
        public List<DogEventModel> GetAllByDogId(int dogId)
        {
            var dogEvents = unitOfWork.DogEventRepository.GetAllByDogId(dogId);
            var dogEventModels = new List<DogEventModel>();
            foreach (var dogEvent in dogEvents)
            {
                dogEventModels.Add(new DogEventModel
                {
                    DogId = dogEvent.DogId,
                    Dog = new DogModel
                    {
                        DogId = dogEvent.Dog.DogId,
                        Name = dogEvent.Dog.Name,
                        GuideIdAndName = new IdNameModel
                        {
                            Id = dogEvent.Dog.Guide.GuideId,
                            Name = $"{dogEvent.Dog.Guide.FirstName} {dogEvent.Dog.Guide.LastName}"
                        }
                    },
                    EventId = dogEvent.EventId,
                    Event = new EventModel
                    {
                        City = dogEvent.Event.City,
                        Date = dogEvent.Event.Date,
                        Description = dogEvent.Event.Description,
                        EventId = dogEvent.Event.EventId,
                        Notes = dogEvent.Event.Notes,
                        StreetOrLocation = dogEvent.Event.StreetOrLocation,
                        Title = dogEvent.Event.Title

                    },
                    LostPerson = dogEvent.LostPerson,
                    Weather = dogEvent.Weather,
                    Notes = dogEvent.Notes
                });
            }
            return dogEventModels;
        }

        [HttpGet("GetAllByEventId")]
        public List<DogEventModel> GetAllByEventId(int eventId)
        {
            var dogEvents = unitOfWork.DogEventRepository.GetAllByEventId(eventId);
            var dogEventModels = new List<DogEventModel>();
            foreach (var dogEvent in dogEvents)
            {
                dogEventModels.Add(new DogEventModel
                {
                    DogId = dogEvent.DogId,
                    Dog = new DogModel
                    {
                        DogId = dogEvent.Dog.DogId,
                        Name = dogEvent.Dog.Name,
                        GuideIdAndName = new IdNameModel
                        {
                            Id = dogEvent.Dog.Guide.GuideId,
                            Name = $"{dogEvent.Dog.Guide.FirstName} {dogEvent.Dog.Guide.LastName}"
                        }
                    },
                    EventId = dogEvent.EventId,
                    Event = new EventModel
                    {
                        City = dogEvent.Event.City,
                        Date = dogEvent.Event.Date,
                        Description = dogEvent.Event.Description,
                        EventId = dogEvent.Event.EventId,
                        Notes = dogEvent.Event.Notes,
                        StreetOrLocation = dogEvent.Event.StreetOrLocation,
                        Title = dogEvent.Event.Title

                    },
                    LostPerson = dogEvent.LostPerson,
                    Weather = dogEvent.Weather,
                    Notes = dogEvent.Notes
                });
            }
            return dogEventModels;
        }
    }
}