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
    public class DogCertificatesController : BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork(new AppDbContext());

        [HttpGet]
        public List<DogCertificateModel> Get()
        {
            var dogCerts = unitOfWork.DogCertificateRepository.GetAll().ToList();
            var dogCertificateModels = new List<DogCertificateModel>();
            foreach(var dogCert in dogCerts)
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

        [HttpGet]
        public DogCertificateModel Get(int dogId, int certificateId)
        {
            var dogCert = unitOfWork.DogCertificateRepository.GetByIds(dogId, certificateId);
            var dogCertModel = new DogCertificateModel
            {
                DogId = dogCert.DogId,
                CertificateId = dogCert.CertificateId,
                AcquiredOn = dogCert.AcquiredOn
            };
            return dogCertModel;
        }

        [HttpPost]
        public IActionResult Post([FromBody]DogCertificateModel obj)
        {
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
            var dogCertificate = unitOfWork.DogCertificateRepository.GetByIds(dogId, certificateId);
            dogCertificate.AcquiredOn = obj.AcquiredOn;
            unitOfWork.Commit();
            return new ObjectResult(new { dogCertificate.DogId, dogCertificate.CertificateId });
        }

        [HttpDelete]
        public IActionResult Delete(int dogId, int certificateId)
        {
            unitOfWork.DogCertificateRepository.Delete(unitOfWork.DogCertificateRepository.GetByIds(dogId, certificateId));
            unitOfWork.Commit();
            return new ObjectResult("DogCertificate deleted successfully!");
        }
    }
}
