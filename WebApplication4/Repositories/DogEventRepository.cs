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
    public class DogEventRepository : Repository<DogEvent>
    {
        protected new DbSet<DogEvent> DbSet;

        public DogEventRepository(DbContext dataContext)
             : base(dataContext)
        {
            DbSet = dataContext.Set<DogEvent>();
        }


        public new IQueryable<DogEvent> SearchFor(Expression<Func<DogEvent, bool>> predicate)
        {
            return DbSet.Include(x => x.Dog).Include(x => x.Event).Where(predicate);
        }

        public new IQueryable<DogEvent> GetAll()
        {
            //return DbSet;
            return DbSet.Include(x => x.Dog).Include(x => x.Event);
        }

        public DogEvent GetByIds(int dogId, int eventId)
        {
            var DogEvent = DbSet.Where(x => x.EventId == eventId && x.DogId == dogId).Include(x => x.Dog).Include(x => x.Event).FirstOrDefault();
            return DogEvent;
        }
    }
}
