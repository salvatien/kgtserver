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
    public class ActionsController : BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork(new AppDbContext());

        [HttpGet]
        public List<DogsServer.Models.Action> Get()
        {
            return unitOfWork.ActionRepository.GetAll().ToList();
        }

        [HttpGet("{id}")]
        public DogsServer.Models.Action Get(int id)
        {
            return unitOfWork.ActionRepository.GetById(id);
        }

        [HttpPost]
        public IActionResult Post([FromBody]DogsServer.Models.Action obj)
        {
            unitOfWork.ActionRepository.Insert(obj);
            unitOfWork.Commit();
            return new ObjectResult("Action added successfully!");
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]DogsServer.Models.Action obj)
        {
            var Action = unitOfWork.ActionRepository.GetById(id);
            Action = obj;
            unitOfWork.Commit();
            return new ObjectResult("Action modified successfully!");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            unitOfWork.ActionRepository.Delete(unitOfWork.ActionRepository.GetById(id));
            unitOfWork.Commit();
            return new ObjectResult("Action deleted successfully!");
        }
    }
}
