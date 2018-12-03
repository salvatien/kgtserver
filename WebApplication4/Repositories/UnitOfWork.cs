using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogsServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DogsServer.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;
        #region Repositories
        public IRepository<Employee> EmployeeRepository =>
           new Repository<Employee>(_dbContext);
        public GuideRepository GuideRepository =>
           new GuideRepository(_dbContext);
        public DogRepository DogRepository =>
           new DogRepository(_dbContext);
        public IRepository<Models.Action> ActionRepository =>
           new Repository<Models.Action>(_dbContext);
        public IRepository<Event> EventRepository =>
           new Repository<Event>(_dbContext);

        #endregion
        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Commit()
        {
            _dbContext.SaveChanges();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
        public void RejectChanges()
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries()
                  .Where(e => e.State != EntityState.Unchanged))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                }
            }
        }
    }
}
