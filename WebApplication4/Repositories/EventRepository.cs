using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace DogsServer.Repositories
{
    public class EventRepository : Repository<DogsServer.Models.Event>
    {
        protected new DbSet<DogsServer.Models.Event> DbSet;

        public EventRepository(DbContext dataContext)
             : base(dataContext)
        {
            DbSet = dataContext.Set<DogsServer.Models.Event>();
        }


        public new IQueryable<DogsServer.Models.Event> SearchFor(Expression<Func<DogsServer.Models.Event, bool>> predicate)
        {
            return DbSet.Include(x => x.GuideEvents).Include(x => x.Dogs).Where(predicate);
        }

        public new IQueryable<DogsServer.Models.Event> GetAll()
        {
            return DbSet.Include(x => x.GuideEvents).Include(x => x.Dogs);
        }

        public DogsServer.Models.Event GetById(int id)
        {
            //return DbSet.Find(id);
            //var dog = DbSet.Where(x => x.DogID == id).Include(x => x.Guide).FirstOrDefault();
            //return dog;
            var guide = DbSet.Where(x => x.EventId == id).Include(x => x.GuideEvents).Include(x => x.Dogs).FirstOrDefault();
            return guide;
        }
    }
}