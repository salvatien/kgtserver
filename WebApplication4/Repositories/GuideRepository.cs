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
            return DbSet.Include(x=> x.Dogs).Include(x=> x.GuideActions).Include(x => x.GuideEvents).Where(predicate);
        }

        public new IQueryable<Guide> GetAll()
        {
            return DbSet.Include(x => x.Dogs).Include(x => x.GuideActions).Include(x => x.GuideEvents);
        }

        public new Guide GetById(int id)
        {
            //return DbSet.Find(id);
            //var dog = DbSet.Where(x => x.DogID == id).Include(x => x.Guide).FirstOrDefault();
            //return dog;
            var guide = DbSet.Where(x => x.GuideID == id).Include(x => x.Dogs).Include(x => x.GuideActions).Include(x => x.GuideEvents).FirstOrDefault();
            return guide;
        }

        public int GetFreeId()
        {
            var id = DbSet.OrderByDescending(u => u.GuideID).FirstOrDefault().GuideID + 1;
            return id;
        }

    }
}
