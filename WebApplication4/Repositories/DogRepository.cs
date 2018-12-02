using System;
//using System.Data.Entity;
//using System.Linq;
//using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using DogsServer.Models;
using System.Linq;
using System.Linq.Expressions;

namespace DogsServer.Repositories
{
    public class DogRepository : Repository<Dog>
    {
        protected new DbSet<Dog> DbSet;

        public DogRepository(DbContext dataContext)
             : base(dataContext)
        {
            DbSet = dataContext.Set<Dog>();
        }
        

        public new IQueryable<Dog> SearchFor(Expression<Func<Dog, bool>> predicate)
        {
            return DbSet.Include(x=> x.Guide).Where(predicate);
        }

        public new IQueryable<Dog> GetAll()
        {
            return DbSet.Include(x => x.Guide);
        }

        public new Dog GetById(int id)
        {
            //var dog = DbSet.Find(id);
            var dog = DbSet.Where(x => x.DogID == id).Include(x => x.Guide).FirstOrDefault();
            return dog;
        }

    }
}
