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
    public class GuideRepository : Repository<Guide>
    {
        protected new DbSet<Guide> DbSet;

        public GuideRepository(DbContext dataContext)
             : base(dataContext)
        {
            DbSet = dataContext.Set<Guide>();
        }


        public new IQueryable<Guide> SearchFor(Expression<Func<Guide, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public new IQueryable<Guide> GetAll()
        {
            return DbSet;
        }

        public new Guide GetById(int id)
        {
            return DbSet.Find(id);
            //var dog = DbSet.Where(x => x.DogID == id).Include(x => x.Guide).FirstOrDefault();
            //return dog;
        }

        public int GetFreeId()
        {
            var id =  DbSet.OrderByDescending(u => u.GuideID).FirstOrDefault().GuideID + 1;
            return id;
        }

    }
}
