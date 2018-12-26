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
    public class DogActionRepository : Repository<DogAction>
    {
        protected new DbSet<DogAction> DbSet;

        public DogActionRepository(DbContext dataContext)
             : base(dataContext)
        {
            DbSet = dataContext.Set<DogAction>();
        }


        public new IQueryable<DogAction> SearchFor(Expression<Func<DogAction, bool>> predicate)
        {
            return DbSet.Include(x => x.Dog).Include(x => x.Action).Where(predicate);
        }

        public new IQueryable<DogAction> GetAll()
        {
            //return DbSet;
            return DbSet.Include(x => x.Dog).Include(x => x.Action);
        }

        public DogAction GetByIds(int dogId, int actionId)
        {
            var DogAction = DbSet.Where(x => x.ActionId == actionId && x.DogId == dogId).Include(x => x.Dog).Include(x => x.Action).FirstOrDefault();
            return DogAction;
        }
    }
}
