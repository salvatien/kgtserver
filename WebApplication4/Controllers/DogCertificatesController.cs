﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dogs.Data.Models;
using DogsServer.Repositories;
using Dogs.Data.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Dogs.Data.DbContexts;
using DogsServer.Services;

namespace DogsServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class DogCertificatesController : BaseController
    {
        private readonly UnitOfWork unitOfWork;
        private readonly AppDbContext appDbContext;

        public DogCertificatesController(IUserService userService, AppDbContext dbContext) : base(userService)  
        {
            appDbContext = dbContext;
            unitOfWork = new UnitOfWork(appDbContext);
        }

        [HttpGet]
        public List<DogCertificateModel> Get()
        {
            var dogCerts = unitOfWork.DogCertificateRepository.GetAll().ToList();
            var dogCertificateModels = new List<DogCertificateModel>();
            foreach (var dogCert in dogCerts)
            {
                var dogCertModel = new DogCertificateModel
                {
                    DogId = dogCert.DogId,
                    CertificateId = dogCert.CertificateId,
                    AcquiredOn = dogCert.AcquiredOn
                };
                dogCertificateModels.Add(dogCertModel);
            }
            return dogCertificateModels;

        }

        [HttpGet("DogCertificate")]
        public DogCertificateModel Get(int dogId, int certificateId)
        {
            var dogCert = unitOfWork.DogCertificateRepository.GetByIds(dogId, certificateId);
            var dogCertModel = new DogCertificateModel
            {
                DogId = dogCert.DogId,
                Dog = new DogModel
                {
                    DogId = dogCert.Dog.DogId,
                    Name = dogCert.Dog.Name,
                    GuideIdAndName = new IdNameModel
                    {
                        Id = dogCert.Dog.Guide.GuideId,
                        Name = $"{dogCert.Dog.Guide.FirstName} {dogCert.Dog.Guide.LastName}"
                    }
                },
                CertificateId = dogCert.CertificateId,
                Certificate = new CertificateModel
                {
                    CertificateId = dogCert.CertificateId,
                    Description = dogCert.Certificate.Description,
                    Name = dogCert.Certificate.Name,
                    Level = dogCert.Certificate.Level,
                    ValidThroughMonths = dogCert.Certificate.ValidThroughMonths
                },
                AcquiredOn = dogCert.AcquiredOn
            };
            return dogCertModel;
        }

        [HttpGet ("GetAllByDogId")]
        public List<DogCertificateModel> GetAllByDogId(int dogId)
        {
            var dogCertificates = unitOfWork.DogCertificateRepository.GetAllByDogId(dogId);
            var dogCertModel = new List<DogCertificateModel>();
            foreach (var dogCert in dogCertificates)
            {
                dogCertModel.Add( new DogCertificateModel
                {
                    DogId = dogCert.DogId,
                    Dog = new DogModel
                    {
                    DogId = dogCert.Dog.DogId,
                    Name = dogCert.Dog.Name,
                    GuideIdAndName = new IdNameModel { Id = dogCert.Dog.Guide.GuideId,
                                                       Name = $"{dogCert.Dog.Guide.FirstName} {dogCert.Dog.Guide.LastName}"}
                    },
                    CertificateId = dogCert.CertificateId,
                    Certificate = new CertificateModel
                    {
                        CertificateId = dogCert.CertificateId,
                        Description = dogCert.Certificate.Description,
                        Name = dogCert.Certificate.Name,
                        Level = dogCert.Certificate.Level,
                        ValidThroughMonths = dogCert.Certificate.ValidThroughMonths
                    },
                    AcquiredOn = dogCert.AcquiredOn
                });
            }
            return dogCertModel;
        }

        [HttpGet("GetAllByCertificateId")]
        public List<DogCertificateModel> GetAllByCertificateId(int certificateId)
        {
            var dogCertificates = unitOfWork.DogCertificateRepository.GetAllByCertificateId(certificateId);
            var dogCertModel = new List<DogCertificateModel>();
            foreach (var dogCert in dogCertificates)
            {
                dogCertModel.Add(new DogCertificateModel
                {
                    DogId = dogCert.DogId,
                    Dog = new DogModel
                    {
                        DogId = dogCert.Dog.DogId,
                        Name = dogCert.Dog.Name,
                        GuideIdAndName = new IdNameModel
                        {
                            Id = dogCert.Dog.Guide.GuideId,
                            Name = $"{dogCert.Dog.Guide.FirstName} {dogCert.Dog.Guide.LastName}"
                        }
                    },
                    CertificateId = dogCert.CertificateId,
                    Certificate = new CertificateModel
                    {
                        CertificateId = dogCert.CertificateId,
                        Description = dogCert.Certificate.Description,
                        Name = dogCert.Certificate.Name,
                        Level = dogCert.Certificate.Level,
                        ValidThroughMonths = dogCert.Certificate.ValidThroughMonths
                    },
                    AcquiredOn = dogCert.AcquiredOn
                });
            }
            return dogCertModel;
        }

        [HttpPost]
        public IActionResult Post([FromBody]DogCertificateModel obj)
        {
            if (!UserService.IsCurrentUserAdmin(User))
                return Forbid();
            var dogCert = new DogCertificate
            {
                DogId = obj.DogId,
                CertificateId = obj.CertificateId,
                AcquiredOn = obj.AcquiredOn
            };
            unitOfWork.DogCertificateRepository.Insert(dogCert);
            unitOfWork.Commit();
            return new ObjectResult(new { dogCert.DogId, dogCert.CertificateId});
        }

        [HttpPut]
        public IActionResult Put(int dogId, int certificateId, [FromBody]DogCertificateModel obj)
        {
            if (!UserService.IsCurrentUserAdmin(User))
                return Forbid();
            var dogCertificate = unitOfWork.DogCertificateRepository.GetByIds(dogId, certificateId);
            dogCertificate.AcquiredOn = obj.AcquiredOn;
            unitOfWork.Commit();
            return new ObjectResult(new { dogCertificate.DogId, dogCertificate.CertificateId });
        }

        [HttpDelete("dogcertificate")]
        public IActionResult Delete(int dogId, int certificateId)
        {
            if (!UserService.IsCurrentUserAdmin(User))
                return Forbid();
            unitOfWork.DogCertificateRepository.Delete(unitOfWork.DogCertificateRepository.GetByIds(dogId, certificateId));
            unitOfWork.Commit();
            return new ObjectResult("DogCertificate deleted successfully!");
        }


    }
}
