using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DogsServer.Repositories;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using DogsServer.Services;
using Dogs.Data.Models;
using Dogs.Data.DbContexts;
using Dogs.Data.DataTransferObjects;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class DogsController : BlobUploaderControllerBase
    {
        private readonly UnitOfWork unitOfWork;
        private readonly AppDbContext appDbContext;

        public DogsController(AppDbContext dbContext,
            IUserService userService, IBlobStorageService blobStorageService) : base(userService, blobStorageService)
        {
            appDbContext = dbContext;
            unitOfWork = new UnitOfWork(appDbContext);
        }

        [HttpGet]
        [AllowAnonymous]
        public List<DogModel> Get()
        {
            var dogs = unitOfWork.DogRepository.GetAll().ToList();
            var dogModels = new List<DogModel>();
            foreach(var dog in dogs)
            {
                var dogModel = new DogModel
                {
                    DateOfBirth = dog.DateOfBirth,
                    DogId = dog.DogId,
                    Breed = dog.Breed,
                    GuideIdAndName = new IdNameModel
                    {
                        Id = dog.Guide != null ? dog.Guide.GuideId : 0,
                        Name = dog.Guide != null ? dog.Guide.FirstName + " " + dog.Guide.LastName : "Pies nie ma jeszcze przewodnika"
                    },
                    Level = (Dogs.Data.DataTransferObjects.Enums.DogLevel)dog.Level,
                    Name = dog.Name,
                    Notes = dog.Notes,
                    PhotoBlobUrl = dog.PhotoBlobUrl,
                    Workmodes = (Dogs.Data.DataTransferObjects.Enums.DogWorkmode?)dog.Workmodes,
                    CertificateIds = dog.DogCertificates?.Select(x=>x.CertificateId).ToList(),
                    EventIds = dog.DogEvents?.Select(x=>x.EventId).ToList(),
                    TrainingIds = dog.DogTrainings?.Select(x=>x.TrainingId).ToList()
                };
                dogModels.Add(dogModel);
            }
            return dogModels;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public DogModel Get(int id)
        {
            var dog = unitOfWork.DogRepository.GetById(id);
            var dogModel = new DogModel
            {
                DateOfBirth = dog.DateOfBirth,
                DogId = dog.DogId,
                Breed = dog.Breed,
                GuideIdAndName = new IdNameModel
                {
                    Id = dog.Guide != null ? dog.Guide.GuideId : 0,
                    Name = dog.Guide != null ? dog.Guide.FirstName + " " + dog.Guide.LastName : "Pies nie ma jeszcze przewodnika"
                },
                Level = (Dogs.Data.DataTransferObjects.Enums.DogLevel)dog.Level,
                Name = dog.Name,
                Notes = dog.Notes,
                PhotoBlobUrl = dog.PhotoBlobUrl,
                Workmodes = (Dogs.Data.DataTransferObjects.Enums.DogWorkmode?)dog.Workmodes,
                CertificateIds = dog.DogCertificates?.Select(x => x.CertificateId).ToList(),
                EventIds = dog.DogEvents?.Select(x => x.EventId).ToList(),
                TrainingIds = dog.DogTrainings?.Select(x => x.TrainingId).ToList()
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
                Level = (Dogs.Data.Models.Enums.DogLevel)dogModel.Level,
                Name = dogModel.Name,
                Notes = dogModel.Notes,
                Workmodes = (Dogs.Data.Models.Enums.DogWorkmode?)dogModel.Workmodes,
                Breed = dogModel.Breed,
                PhotoBlobUrl = dogModel.PhotoBlobUrl
            };
            
            unitOfWork.DogRepository.Insert(dog);
            unitOfWork.Commit();
            var x = dog.DogId;
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
                dog.Breed = updatedDog.Breed;
                dog.DateOfBirth = updatedDog.DateOfBirth;
                dog.Level = (Dogs.Data.Models.Enums.DogLevel)updatedDog.Level;
                dog.Notes = updatedDog.Notes;
                dog.Workmodes = (Dogs.Data.Models.Enums.DogWorkmode?)updatedDog.Workmodes;
                dog.PhotoBlobUrl = updatedDog.PhotoBlobUrl;
                if (updatedDog.GuideIdAndName != null)
                {
                    if (dog.Guide == null || dog.Guide.GuideId != updatedDog.GuideIdAndName.Id)
                    {
                        var guide = unitOfWork.GuideRepository.GetById(updatedDog.GuideIdAndName.Id);
                        dog.Guide = guide;
                    }
                }
                unitOfWork.Commit();
                return new ObjectResult(dog.DogId);
            }
            catch (Exception e)
            {
                return new ObjectResult(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var dog = unitOfWork.DogRepository.GetById(id);
            if (!UserService.IsCurrentUserAdmin(User) && UserService.GetCurrentUserId(User) != dog.Guide.GuideId)
            {
                return Forbid();
            }
                
            unitOfWork.DogRepository.Delete(unitOfWork.DogRepository.GetById(id));
            unitOfWork.Commit();
            return new ObjectResult("Dog deleted successfully!");
        }


        [HttpPost("Upload")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
            return await Upload("images");
        }       
    }
}
