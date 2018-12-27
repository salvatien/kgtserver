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
    public class CertificatesController : BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork(new AppDbContext());

        [HttpGet]
        public List<CertificateModel> Get()
        {
            var certs = unitOfWork.CertificateRepository.GetAll().ToList();
            var certificateModels = new List<CertificateModel>();
            foreach(var cert in certs)
            {
                var certModel = new CertificateModel
                {
                    CertificateId = cert.CertificateId,
                    Name = cert.Name,
                    Level = cert.Level,
                    Description = cert.Description,
                    ValidThrough = cert.ValidThrough,
                    DogIds = cert.DogCertificates.Select(x => x.DogId).ToList()
                };
                certificateModels.Add(certModel);
            }
            return certificateModels;
        }

        [HttpGet("{id}")]
        public CertificateModel Get(int id)
        {
            var cert =  unitOfWork.CertificateRepository.GetById(id);
            
            var certModel = new CertificateModel
            {
                CertificateId = cert.CertificateId,
                Name = cert.Name,
                Level = cert.Level,
                Description = cert.Description,
                ValidThrough = cert.ValidThrough,
                DogIds = cert.DogCertificates.Select(x => x.DogId).ToList()
            };
            return certModel;
        }

        [HttpPost]
        public IActionResult Post([FromBody]CertificateModel obj)
        {
            var cert = new Certificate
            {
                Name = obj.Name,
                Level = obj.Level,
                Description = obj.Description,
                ValidThrough = obj.ValidThrough
            };
            unitOfWork.CertificateRepository.Insert(cert);
            unitOfWork.Commit();
            return new ObjectResult(cert.CertificateId);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]CertificateModel obj)
        {

            var certificate = unitOfWork.CertificateRepository.GetById(id);
            var newCert = new Certificate
            {
                CertificateId = certificate.CertificateId,
                Name = obj.Name,
                Level = obj.Level,
                Description = obj.Description,
                ValidThrough = obj.ValidThrough
            };
            certificate = newCert;
            unitOfWork.Commit();
            return new ObjectResult(newCert.CertificateId);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            unitOfWork.CertificateRepository.Delete(unitOfWork.CertificateRepository.GetById(id));
            unitOfWork.Commit();
            return new ObjectResult("Certificate deleted successfully!");
        }
    }
}
