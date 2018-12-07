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
    public class EmployeeController : BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork(new AppDbContext());

        [HttpGet]
        public List<Employee> Get()
        {
            return unitOfWork.EmployeeRepository.GetAll().ToList();
        }

        [HttpGet("{id}")]
        public Employee Get(int id)
        {
            return unitOfWork.EmployeeRepository.GetById(id);
        }

        [HttpPost]
        public IActionResult Post([FromBody]Employee obj)
        {
            unitOfWork.EmployeeRepository.Insert(obj);
            unitOfWork.Commit();
            return new ObjectResult("Employee added successfully!");
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Employee obj)
        {
            var employee = unitOfWork.EmployeeRepository.GetById(id);
            employee = obj;
            unitOfWork.Commit();
            return new ObjectResult("Employee modified successfully!");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            unitOfWork.EmployeeRepository.Delete(unitOfWork.EmployeeRepository.GetById(id));
            unitOfWork.Commit();
            return new ObjectResult("Employee deleted successfully!");
        }
    }
}
