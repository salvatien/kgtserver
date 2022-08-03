using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DogsServer.Models;
using DogsServer.Repositories;
using Dogs.ViewModels.Data.Models;
using Microsoft.AspNetCore.Authorization;
using DogsServer.DbContexts;
using DogsServer.Services;

namespace DogsServer.Controllers
{
    [Route("api/[controller]")]
    public class CertificatesController : BaseController
    {
        private readonly UnitOfWork unitOfWork;
        private readonly AppDbContext appDbContext;

        public CertificatesController(IUserService userService, AppDbContext dbContext) : base(userService)
        {
            appDbContext = dbContext;
            unitOfWork = new UnitOfWork(appDbContext);
        }

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
                    ValidThroughMonths = cert.ValidThroughMonths,
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
                ValidThroughMonths = cert.ValidThroughMonths,
                DogIds = cert.DogCertificates.Select(x => x.DogId).ToList()
            };
            return certModel;
        }
        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody]CertificateModel obj)
        {
            if (!UserService.IsCurrentUserAdmin(User))
                return Forbid();
            var cert = new Certificate
            {
                Name = obj.Name,
                Level = obj.Level,
                Description = obj.Description,
                ValidThroughMonths = obj.ValidThroughMonths
            };
            unitOfWork.CertificateRepository.Insert(cert);
            unitOfWork.Commit();
            return new ObjectResult(cert.CertificateId);
        }
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]CertificateModel obj)
        {
            if (!UserService.IsCurrentUserAdmin(User))
                return Forbid();
            var certificate = unitOfWork.CertificateRepository.GetById(id);
            certificate.Name = obj.Name;
            certificate.Level = obj.Level;
            certificate.Description = obj.Description;
            certificate.ValidThroughMonths = obj.ValidThroughMonths;
            unitOfWork.Commit();
            return new ObjectResult(certificate.CertificateId);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!UserService.IsCurrentUserAdmin(User))
                return Forbid();
            unitOfWork.CertificateRepository.Delete(unitOfWork.CertificateRepository.GetById(id));
            unitOfWork.Commit();
            return new ObjectResult("Certificate deleted successfully!");
        }
    }
}
