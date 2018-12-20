using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace DogsServer.Repositories
{
    public class ActionRepository : Repository<DogsServer.Models.Action>
    {
        protected new DbSet<DogsServer.Models.Action> DbSet;

        public ActionRepository(DbContext dataContext)
             : base(dataContext)
        {
            DbSet = dataContext.Set<DogsServer.Models.Action>();
        }


        public new IQueryable<DogsServer.Models.Action> SearchFor(Expression<Func<DogsServer.Models.Action, bool>> predicate)
        {
            return DbSet.Include(x => x.GuideActions).Include(x => x.DogActions).Where(predicate);
        }

        public new IQueryable<DogsServer.Models.Action> GetAll()
        {
            return DbSet.Include(x => x.GuideActions).Include(x => x.DogActions);
        }

        public DogsServer.Models.Action GetById(int id)
        {
            //return DbSet.Find(id);
            //var dog = DbSet.Where(x => x.DogID == id).Include(x => x.Guide).FirstOrDefault();
            //return dog;
            var action = DbSet.Where(x => x.ActionID == id).Include(x => x.GuideActions).Include(x => x.DogActions).FirstOrDefault();
            return action;
        }
    }
}