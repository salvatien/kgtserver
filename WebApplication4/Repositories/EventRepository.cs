using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using Dogs.Data.Models;

namespace DogsServer.Repositories
{
    public class EventRepository : Repository<Event>
    {
        protected new DbSet<Event> DbSet;

        public EventRepository(DbContext dataContext)
             : base(dataContext)
        {
            DbSet = dataContext.Set<Event>();
        }


        public new IQueryable<Event> SearchFor(Expression<Func<Event, bool>> predicate)
        {
            return DbSet.Include(x => x.GuideEvents).Include(x => x.DogEvents).Where(predicate);
        }

        public new IQueryable<Event> GetAll()
        {
            return DbSet.Include(x => x.GuideEvents).Include(x => x.DogEvents);
        }

        public Event GetById(int id)
        {
            //return DbSet.Find(id);
            //var dog = DbSet.Where(x => x.DogID == id).Include(x => x.Guide).FirstOrDefault();
            //return dog;
            var guide = DbSet.Where(x => x.EventId == id).Include(x => x.GuideEvents).Include(x => x.DogEvents).FirstOrDefault();
            return guide;
        }
    }
}