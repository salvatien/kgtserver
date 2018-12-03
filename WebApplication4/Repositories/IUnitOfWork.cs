using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogsServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DogsServer.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<Employee> EmployeeRepository { get; }
        IRepository<Guide> GuideRepository { get; }
        DogRepository DogRepository { get; }
        IRepository<Models.Action> ActionRepository { get; }
        IRepository<Models.Event> EventRepository { get; }

        /// <summary>
        /// Commits all changes
        /// </summary>
        void Commit();
        /// <summary>
        /// Discards all changes that has not been commited
        /// </summary>
        void RejectChanges();
        void Dispose();
    }
    
}
